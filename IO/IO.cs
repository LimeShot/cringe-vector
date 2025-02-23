using System.Text;
namespace IO.Method;
interface IOutput {
   public void Export(); // функция, берущая класс с фигурами
}

interface IInput {
   public void Import(); // функция, возвращающая класс с фигурами
}

public class OutputToSVG : IOutput {
   public string Path;
   public OutputToSVG(string path) {
      Path = path;
   }
   public void Export() {
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
   public void Export(Canvas Field) {
      using (FileStream fstream = new FileStream(Path, FileMode.OpenOrCreate)) {   
         width = Field.width; //Ширина
         height = Field.height; //Высота
         Shapes = Field.Shapes; //Массив фигур
         string text = "";
         foreach(IShape Shape in Shapes){
            text += " "+Shape.Icon;
            text += " "+Shape.Translate.X.ToString();
            text += " "+Shape.Translate.Y.ToString();
            text += " "+Shape.Rotate.ToString();
            switch (Shape.Icon){
               case "Rectangle.png":
                  text += " "+Shape.Size.X.ToString();
                  text += " "+Shape.Size.Y.ToString();
                  break;
               case "Line.png":
                  text +=" "+Shape.Point1.X.ToString();
                  text +=" "+Shape.Point1.Y.ToString();
                  text +=" "+Shape.Point2.X.ToString();
                  text +=" "+Shape.Point2.Y.ToString();
                  break;
               case "Ellipse.png":
                  text += " "+Shape.Size.X.ToString();
                  text += " "+Shape.Size.Y.ToString();
                  break;
               default://Здесь видимо polygon
                  //Дописать
                  break;
            }
            text += " "+Shape.Style.ColorOutLine.X.ToString();
            text += " "+Shape.Style.ColorOutLine.Y.ToString();
            text += " "+Shape.Style.ColorOutLine.Z.ToString();
            text += " "+Shape.Style.ColorFill.X.ToString();
            text += " "+Shape.Style.ColorFill.Y.ToString();
            text += " "+Shape.Style.ColorFill.Z.ToString();
            text += " "+Shape.Style.Fill.ToString();
            text += " "+Shape.Style.Visible.ToString();
            text+="\n";
         // действия цикла
         }
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
