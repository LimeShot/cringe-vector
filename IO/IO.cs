using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
namespace CringeCraft.IO;
public interface IExporter {
    public void Export(string path, ICanvas Field); // функция, берущая класс с фигурами
}

public interface IImporter {
    public Tuple<ObservableCollection<IShape>, float, float> Import(string path); // функция, возвращающая класс с фигурами
}

public class ExportToCRNG : IExporter {
    public void Export(string path, ICanvas Field) {
        // Настройки сериализации
        //var data = new{Shapes, Height, Width};
        var settings = new JsonSerializerSettings {
            ContractResolver = new PrivateSetterContractResolver(),
            Converters = new List<JsonConverter> { new Vector2Converter(), new Vector3Converter() },
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };
        if (!File.Exists(path)) {
            File.Create(path);
        }
        // Сериализация в JSON
        string json = JsonConvert.SerializeObject(Field, settings);
        File.WriteAllText(path, json);        
    }
}

public class ImportFromCRNG : IImporter {
    public Tuple<ObservableCollection<IShape>, float, float> Import(string path) {
        // Настройки сериализации
        var settings = new JsonSerializerSettings {
            ContractResolver = new PrivateSetterContractResolver(),
            Converters = new List<JsonConverter> { new Vector2Converter(), new Vector3Converter() },
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };
        string json = File.ReadAllText(path);
        var jObject = JObject.Parse(json);
        var shapes = jObject["Shapes"].ToObject<ObservableCollection<IShape>>(JsonSerializer.Create(settings));
        float width = jObject["Width"].Value<float>();
        float height = jObject["Height"].Value<float>();
        var data = Tuple.Create(shapes, width, height);
        Console.WriteLine(width);
        Console.WriteLine(shapes.Count);
        return data;
        
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
                line = $"<rect class=\"st0\" x=\"{Math.Min(shape.Nodes[0].X, shape.Nodes[2].X)}\" y=\"{Math.Max(shape.Nodes[0].Y, shape.Nodes[2].Y) * (-1)}\" width=\"{Math.Abs(shape.Nodes[0].X - shape.Nodes[2].X)}\" height=\"{Math.Abs(shape.Nodes[0].Y - shape.Nodes[2].Y)}\" transform=\"translate({shape.Translate.X}, {shape.Translate.Y * (-1)}) rotate({shape.Rotate})\"/>";
                break;
            case "CringeCraft.GeometryDash.Shape.Ellipse":
                line = $"<ellipse class=\"st0\" cx=\"{shape.Translate.X}\" cy=\"{shape.Translate.Y * (-1)}\" rx=\"{Math.Abs(shape.Nodes[0].X - shape.Nodes[2].X) / 2}\" ry=\"{Math.Abs(shape.Nodes[0].Y - shape.Nodes[2].Y) / 2}\" transform=\"rotate({shape.Rotate})\"/>";
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