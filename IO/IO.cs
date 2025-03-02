using System.Text;
using System.Text.Json;
using CringeCraft.GeometryDash.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.IO;
interface IOutput {
   public void Export(ICanvas Field); // функция, берущая класс с фигурами
}

interface IInput {
   public void Import(); // функция, возвращающая класс с фигурами
}

public class OutputToSVG : IOutput {
   public string Path;
   public OutputToSVG(string path) {
      Path = path;
   }
   
   public void Export(ICanvas Field) {
      using (FileStream fstream = new FileStream(Path, FileMode.OpenOrCreate)) {
        // Настройки сериализации
        var options = new JsonSerializerOptions {
            WriteIndented = true, 
            Converters = { new ShapeConverter() } // Добавляем кастомный конвертер
        };

        // Сериализация в JSON
        string json = JsonSerializer.Serialize(Field, options);
        
         if (!File.Exists(Path)) {
            File.Create(Path);
         }
         // преобразуем строку в байты
         byte[] buffer = Encoding.Default.GetBytes(json);
         // запись массива байтов в файл
         fstream.WriteAsync(buffer, 0, buffer.Length);
         Console.WriteLine("Текст записан в файл");
      }
   }
}
