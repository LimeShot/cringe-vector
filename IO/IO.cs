using System.Text;

using CringeCraft.GeometryDash.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.IO.Method;
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
         string text = "<svg>\n</svg>";
         if (!File.Exists(Path)) {
            File.Create(Path);
         }
         // преобразуем строку в байты
         byte[] buffer = Encoding.Default.GetBytes(text);
         // запись массива байтов в файл
         fstream.WriteAsync(buffer, 0, buffer.Length);
         Console.WriteLine("Текст записан в файл");
      }
   }
}
public class OutputToCRNG : IOutput {
   public string Path;
   public OutputToCRNG(string path) {
      Path = path;
   }
   public void Export(ICanvas Field) {
   }
}
