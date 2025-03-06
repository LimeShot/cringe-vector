using System.Text;
using Newtonsoft.Json;
using CringeCraft.GeometryDash;
namespace CringeCraft.IO;
interface IOutput {
    public void Export(ICanvas Field); // функция, берущая класс с фигурами
}

interface IInput {
    public void Import(); // функция, возвращающая класс с фигурами
}

public class OutputToCRNG : IOutput {
    public string Path;
    public OutputToCRNG(string path) {
        Path = path;
    }
    
    public void Export(ICanvas Field) {
        using (FileStream fstream = new FileStream(Path, FileMode.OpenOrCreate)) {

            // Настройки сериализации
            var settings = new JsonSerializerSettings {
                ContractResolver = new PrivateSetterContractResolver(),
                Converters = new List<JsonConverter> { new Vector2Converter(), new Vector3Converter() },
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };

            // Сериализация в JSON
            string json = JsonConvert.SerializeObject(Field, settings);

            if (!File.Exists(Path)) {
                File.Create(Path);
            }
            // преобразуем строку в байты
            byte[] buffer = Encoding.Default.GetBytes(json);
            // запись массива байтов в файл
            fstream.WriteAsync(buffer, 0, buffer.Length);            
        }
    }
}
public class OutputToSVG : IOutput {
    public string Path;
    public OutputToSVG(string path) {
        Path = path;
    }
    public void Export(ICanvas Field) {
        using (FileStream fstream = new FileStream(Path, FileMode.OpenOrCreate)) { }
            
    }
}
