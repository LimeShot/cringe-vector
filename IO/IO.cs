using System.Text;
using Newtonsoft.Json;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using System.Reflection.Metadata;
namespace CringeCraft.IO;
public interface IExporter {
    public void Export(string path, ICanvas field); // функция, берущая класс с фигурами
}

public interface IImporter {
    public void Import(); // функция, возвращающая класс с фигурами
}

public class ExportToCRNG : IExporter {
    public void Export(string path, ICanvas Field) {
        using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate)) {
            // Настройки сериализации
            var settings = new JsonSerializerSettings {
                ContractResolver = new PrivateSetterContractResolver(),
                Converters = new List<JsonConverter> { new Vector2Converter(), new Vector3Converter() },
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };

            // Сериализация в JSON
            string json = JsonConvert.SerializeObject(Field, settings);

            if (!File.Exists(path)) {
                File.Create(path);
            }
            // преобразуем строку в байты
            byte[] buffer = Encoding.Default.GetBytes(json);
            // запись массива байтов в файл
            fstream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}

public class ExportToSVG : IExporter {


    private string shapeToSVG(IShape shape, float height) {
        string line;
        Console.WriteLine(shape.GetType().ToString());
        switch (shape.GetType().ToString()) {
            case "CringeCraft.GeometryDash.Shape.Line":
                line = $"<line class=\"st0\" x1=\"{shape.BoundingBox[1].X}\" y1=\"{shape.BoundingBox[1].Y * (-1)}\" x2=\"{shape.BoundingBox[3].X}\" y2=\"{shape.BoundingBox[3].Y * (-1)}\"/>";
                break;
            case "CringeCraft.GeometryDash.Shape.Rectangle":
                line = $"<rect class=\"st0\" x=\"{shape.Nodes[0].X}\" y=\"{shape.Nodes[0].Y}\" width=\"{Math.Abs(shape.Nodes[0].X - shape.Nodes[1].X)}\" height=\"{Math.Abs(shape.Nodes[0].Y - shape.Nodes[3].Y)}\" transform=\"rotate({shape.Rotate})\"/>";
                break;
            case "CringeCraft.GeometryDash.Shape.Ellipse":
                line = $"<ellipse class=\"st0\" cx=\"{shape.Translate.X}\" cy=\"{shape.Translate.X}\" rx=\"{Math.Abs(shape.Nodes[0].X - shape.Nodes[1].X)}\" ry=\"{Math.Abs(shape.Nodes[0].Y - shape.Nodes[3].Y)}\" transform=\"rotate({shape.Rotate})\"/>";
                break;
            default:
                throw new Exception("Это не просто кринж, это не живет в IShape");
        }
        return line;
    }
    public void Export(string path, ICanvas Field) {
        var fileInfo = new FileInfo(path);
        var fileMode = fileInfo.Exists ? FileMode.Truncate : FileMode.CreateNew; //FileStream fs = File.Open(path, fileMode
        using (FileStream fstream = File.Open(path, fileMode)) {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string text = $"<svg width=\"{Field.Width}\" height=\"{Field.Height}\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"{-Field.Width / 2} {-Field.Height / 2} {Field.Width} {Field.Height}\">\n";
            text += "<defs>\n<style>\n.st0 {\nstroke: #000000;\nfill: blue;\n}\n</style>\n</defs>\n";
            for (int i = 0; i < Field.Shapes.Count; i++) {
                text += $"{shapeToSVG(Field.Shapes[i], Field.Height)}\n";
            }
            text += "</svg>";
            byte[] buffer = Encoding.Default.GetBytes(text);
            fstream.Flush();
            fstream.WriteAsync(buffer, 0, buffer.Length);

        }

    }
}
