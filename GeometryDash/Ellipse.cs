using OpenTK.Mathematics;

using GeometryDash.Shape;

namespace GeometryDash.Shape.Ellipse;

public class Ellipse : Shape {
    public Vector2 Size { set; get; }

    public Ellipse() : base() {
        Size = new(1.0f, 1.0f);
    }

    public Ellipse(float x, float y, Vector2 translate, float rotate, float scale, Vector3 color, bool fill, bool visible)
                    : base(translate, rotate, scale, color, fill, visible) {
        Size = new(x, y);
    }

    public override Vector2[] GetLines() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у элипса
        */
        return [];
    }

    public override Vector2[] GetTriangles() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у элипса
        */
        return [];
    }
}