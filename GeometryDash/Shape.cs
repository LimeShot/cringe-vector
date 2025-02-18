using System.Data;
using System.Runtime.CompilerServices;

using OpenTK.Mathematics;

namespace GeometryDash.Shape;

public abstract class Shape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public float Scale { set; get; }
    public ShapeStyle Style { private set; get; }

    public class ShapeStyle {
        public Vector3 Color { set; get; }
        public bool Fill { set; get; }
        public bool Visible { set; get; }

        public ShapeStyle() {
            Color = (0.0f, 0.0f, 0.0f);
            Fill = true;
            Visible = true;
        }

        public ShapeStyle(Vector3 color, bool fill, bool visible) {
            Color = color;
            Fill = fill;
            Visible = visible;
        }
    }

    protected Shape() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Scale = 1.0f;
        Style = new ShapeStyle();
    }
    protected Shape(Vector2 translate, float rotate, float scale, Vector3 color, bool fill, bool visible) {
        Translate = translate;
        Rotate = rotate;
        Scale = scale;
        Style = new ShapeStyle(color, fill, visible);
    }
    /*Метод возвращает линии фигуры(без заполнения(Fill=false))
    необходимые для отрисовки*/
    public abstract Vector2[] GetLines();

    /*Метод возвращает треугольники(с заполнением(Fill=true))
    необходимые для отрисовки фигуры*/
    public abstract Vector2[] GetTriangles();
}