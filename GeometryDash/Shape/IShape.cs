using System.Data;
using System.Runtime.CompilerServices;
using System.ComponentModel;

using OpenTK.Mathematics;

namespace CringeCraft.GeometryDash.Shape;

public interface IShape: INotifyPropertyChanged {
    
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public ShapeStyle Style { set; get; }

    //Метод возвращает линии фигуры(без заполнения(Fill=false))
    //необходимые для отрисовки
    public float[] GetLineVertices();

    //Метод возвращает треугольники(с заполнением(Fill=true))
    //необходимые для отрисовки фигуры
    public float[] GetTriangleVertices();

    public Vector2[] GetBoundingBox();

    //Метод для проверки принадлежности точки фигуре 
    public bool ContainsPoint(Vector2 point);
}