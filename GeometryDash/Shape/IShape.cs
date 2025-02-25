using System.Data;
using System.Runtime.CompilerServices;

using OpenTK.Mathematics;

namespace CringeCraft.GeometryDash.Shape;

public interface IShape {
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

    // Метод для перемещения фигуры на основе разницы между двумя точками
    public void Move(float x1, float y1, float x2, float y2);
}

}