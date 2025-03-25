using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using Microsoft.VisualBasic;
using Svg;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
namespace CringeCraft.IO;
public interface IExporter {
    public void Export(string path, ICanvas Field); // функция, берущая класс с фигурами
}

public interface IImporter {
    public Tuple<ObservableCollection<IShape>?, float, float> Import(string path); // функция, возвращающая класс с фигурами
}

public class ExportToCRNG : IExporter {
    public void Export(string path, ICanvas Field) {
        // Настройки сериализации
        var settings = new JsonSerializerSettings {
            ContractResolver = new PrivateSetterContractResolver(),
            Converters = new List<JsonConverter> { new Vector2Converter(), new Vector3Converter() },
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };
        var fileInfo = new FileInfo(path);
        var fileMode = fileInfo.Exists ? FileMode.Truncate : FileMode.CreateNew; //FileStream fs = File.Open(path, fileMode
        using (FileStream fstream = File.Open(path, fileMode)) {
            // Сериализация в JSON
            string json = JsonConvert.SerializeObject(Field, settings);
            byte[] buffer = Encoding.Default.GetBytes(json);
            fstream.Flush();
            fstream.WriteAsync(buffer, 0, buffer.Length);
        }

    }
}

public class ImportFromCRNG : IImporter {
    public Tuple<ObservableCollection<IShape>?, float, float> Import(string path) {
        // Настройки сериализации
        var settings = new JsonSerializerSettings {
            ContractResolver = new PrivateSetterContractResolver(),
            Converters = new List<JsonConverter> { new Vector2Converter(), new Vector3Converter() },
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };
        string json = File.ReadAllText(path);
        var jObject = JObject.Parse(json);
        ObservableCollection<IShape>? shapes = new();

        if (jObject["Shapes"] is JArray shapesArray) {
            shapes = shapesArray.ToObject<ObservableCollection<IShape>>(JsonSerializer.Create(settings));
        } else {
            Console.WriteLine("Ошибка: ключ 'Shapes' не найден или не является массивом");
        }
        float width = jObject["Width"]?.Value<float>() ?? 0.0f;
        float height = jObject["Height"]?.Value<float>() ?? 0.0f;
        var data = Tuple.Create(shapes, width, height);

        return data;

    }
}


public class ExportToSVG : IExporter {


    private string shapeToSVG(IShape shape, float height, int styleNumber) {
        string line;
        switch (shape.GetType().ToString()) {
            case "CringeCraft.GeometryDash.Shape.Line":
                line = $"<line class=\"st{styleNumber}\" x1=\"{shape.BoundingBox[0].X}\" y1=\"{shape.BoundingBox[0].Y * (-1)}\" x2=\"{shape.BoundingBox[1].X}\" y2=\"{shape.BoundingBox[1].Y * (-1)}\"/>";
                break;
            case "CringeCraft.GeometryDash.Shape.Rectangle":
                line = $"<rect class=\"st{styleNumber}\" x=\"{Math.Min(shape.Nodes[0].X, shape.Nodes[2].X)}\" y=\"{Math.Max(shape.Nodes[0].Y, shape.Nodes[2].Y) * (-1)}\" width=\"{Math.Abs(shape.Nodes[0].X - shape.Nodes[2].X)}\" height=\"{Math.Abs(shape.Nodes[0].Y - shape.Nodes[2].Y)}\" transform=\"translate({shape.Translate.X}, {shape.Translate.Y * (-1)}) rotate({shape.Rotate})\"/>";
                break;
            case "CringeCraft.GeometryDash.Shape.Ellipse":
                line = $"<ellipse class=\"st{styleNumber}\" cx=\"{shape.Translate.X}\" cy=\"{shape.Translate.Y * (-1)}\" rx=\"{Math.Abs(shape.Nodes[0].X - shape.Nodes[2].X) / 2}\" ry=\"{Math.Abs(shape.Nodes[0].Y - shape.Nodes[2].Y) / 2}\" transform=\"rotate({shape.Rotate})\"/>";
                break;
            case "CringeCraft.GeometryDash.Shape.Polygon":
                line = $"<polygon class=\"st{styleNumber}\"  points=\"";
                for (int i = 0; i < shape.Nodes.Length; i++) {
                    line += $"{shape.Nodes[i].X},{shape.Nodes[i].Y} ";
                }
                line += $"\" transform=\"translate({shape.Translate.X}, {shape.Translate.Y * (-1)}) rotate({shape.Rotate})\"/>";
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
            string textHead = $"<svg width=\"{Field.Width}\" height=\"{Field.Height}\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"{-Field.Width / 2} {-Field.Height / 2} {Field.Width} {Field.Height}\">\n";
            string textStyle = "";
            string textShape = "";
            List<ShapeStyle> style = new List<ShapeStyle>();
            textStyle += "<defs>\n<style>\n";
            for (int i = 0; i < Field.Shapes.Count; i++) {
                int j = 0;
                for (; j < style.Count; j++)
                    if (style[j] == Field.Shapes[i].Style)
                        break;
                if (j == style.Count) {
                    if (Field.Shapes[i].Style.Fill)
                        textStyle += $".st{j} {{\nstroke: rgb({Field.Shapes[i].Style.ColorOutline.X * 255}, {Field.Shapes[i].Style.ColorOutline.Y * 255}, {Field.Shapes[i].Style.ColorOutline.Z * 255});\nfill: rgb({Field.Shapes[i].Style.ColorFill.X * 255}, {Field.Shapes[i].Style.ColorFill.Y * 255}, {Field.Shapes[i].Style.ColorFill.Z * 255});\n}}\n";
                    else
                        textStyle += $".st{j} {{\nstroke: rgb({Field.Shapes[i].Style.ColorOutline.X * 255}, {Field.Shapes[i].Style.ColorOutline.Y * 255}, {Field.Shapes[i].Style.ColorOutline.Z * 255});\nfill: none;\n}}\n";
                    style.Add(Field.Shapes[i].Style);
                }
                textShape += $"{shapeToSVG(Field.Shapes[i], Field.Height, j)}\n";// передем номер стиля
            }
            textStyle += "</style>\n</defs>\n";
            textShape += "</svg>";
            byte[] buffer = Encoding.Default.GetBytes(textHead + textStyle + textShape);
            fstream.Flush();
            fstream.WriteAsync(buffer, 0, buffer.Length);

        }

    }

    public void ExportStream(Stream outputStream, ICanvas Field) {
        // Устанавливаем культуру для корректного форматирования чисел
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        // Создаем текстовый писатель для записи в поток
        using (var writer = new StreamWriter(outputStream, Encoding.UTF8, 1024, leaveOpen: true)) {
            // Записываем заголовок SVG
            writer.WriteLine($"<svg width=\"{Field.Width}\" height=\"{Field.Height}\" " +
                $"xmlns=\"http://www.w3.org/2000/svg\" " +
                $"viewBox=\"{-Field.Width / 2} {-Field.Height / 2} {Field.Width} {Field.Height}\">");

            // Записываем стили
            writer.WriteLine("<defs>");
            writer.WriteLine("<style>");

            var style = new List<ShapeStyle>();
            foreach (var shape in Field.Shapes) {
                int styleIndex = style.IndexOf(shape.Style);
                if (styleIndex == -1) {
                    styleIndex = style.Count;
                    style.Add(shape.Style);

                    var outlineColor = shape.Style.ColorOutline;
                    var fillColor = shape.Style.ColorFill;

                    writer.Write($".st{styleIndex} {{");
                    writer.Write($"stroke: rgb({outlineColor.X * 255}, {outlineColor.Y * 255}, {outlineColor.Z * 255});");

                    if (shape.Style.Fill)
                        writer.Write($"fill: rgb({fillColor.X * 255}, {fillColor.Y * 255}, {fillColor.Z * 255});");
                    else
                        writer.Write("fill: none;");

                    writer.WriteLine("}");
                }
            }

            writer.WriteLine("</style>");
            writer.WriteLine("</defs>");

            // Записываем фигуры
            for (int i = 0; i < Field.Shapes.Count; i++) {
                int styleIndex = style.IndexOf(Field.Shapes[i].Style);
                writer.WriteLine(shapeToSVG(Field.Shapes[i], Field.Height, styleIndex));
            }

            // Закрываем SVG
            writer.WriteLine("</svg>");
        }
    }

}

public class ExportToPNG : IExporter {
    public void ExportWithFile(string path, ICanvas Field) {

        int lastSlashIndex = path.LastIndexOf('\\');
        string pathWithoutFileName = "";

        try {
            if (lastSlashIndex < 0) {
                throw new InvalidOperationException("Символ '\\' не найден в пути.");
            }
            pathWithoutFileName = path.Substring(0, lastSlashIndex);
        } catch (InvalidOperationException ex) {
            Console.WriteLine(ex.Message);
        }
        // Временный файл для .svg
        //string tempFilePath = pathWithoutFileName + Path.GetTempFileName();
        string tempFilePath = Path.GetTempFileName();
        string svgFilePath = Path.ChangeExtension(tempFilePath, ".svg");

        var tmpExp = new ExportToSVG();
        tmpExp.Export(svgFilePath, Field);

        var svgDocument = SvgDocument.Open(svgFilePath);

        // Преобразование в Bitmap

        int width = (int)svgDocument.Width.Value;
        int height = (int)svgDocument.Height.Value;

        // Создаём Bitmap с белым фоном
        using (var bitmap = new Bitmap(width, height))

        //var bitmap = svgDocument.Draw();

        // Сохранение в PNG
        using (var graphics = Graphics.FromImage(bitmap)) {
            // Заливаем фон белым цветом
            graphics.Clear(Color.White);

            // Рисуем SVG на Bitmap
            svgDocument.Draw(graphics);

            // Сохраняем результат в PNG
            bitmap.Save(path, ImageFormat.Png);
        }

        File.Delete(tempFilePath);

    }

    public void Export(string path, ICanvas Field) {
        int lastSlashIndex = path.LastIndexOf('\\');
        string pathWithoutFileName = lastSlashIndex >= 0 ? path.Substring(0, lastSlashIndex) : "";

        using (var memoryStream = new MemoryStream()) {
            // Экспортируем SVG в поток памяти
            var tmpExp = new ExportToSVG();
            tmpExp.ExportStream(memoryStream, Field);

            // Сбрасываем позицию потока для чтения
            memoryStream.Position = 0;

            // Загружаем SVG из потока
            var svgDocument = SvgDocument.Open<SvgDocument>(memoryStream);

            // Создаем Bitmap с белым фоном
            using (var bitmap = new Bitmap((int)svgDocument.Width.Value, (int)svgDocument.Height.Value))
            using (var graphics = Graphics.FromImage(bitmap)) {
                graphics.Clear(Color.White);
                svgDocument.Draw(graphics);
                bitmap.Save(path, ImageFormat.Png);
            }
        }
    }
}