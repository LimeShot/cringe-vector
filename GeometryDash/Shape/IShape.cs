using System.Data;
using System.Runtime.CompilerServices;

using OpenTK.Mathematics;

namespace CringeCraft.GeometryDash.Shape;

public interface IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public float Scale { set; get; }
    public ShapeStyle Style { set; get; }

    //Метод возвращает линии фигуры(без заполнения(Fill=false))
    //необходимые для отрисовки
    public Vector2[] GetLineVertices();

    //Метод возвращает треугольники(с заполнением(Fill=true))
    //необходимые для отрисовки фигуры
    public Vector2[] GetTriangleVertices();

    public Vector2[] GetBoundingBox();
}