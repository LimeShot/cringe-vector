using OpenTK.Mathematics;

using GeometryDash.Shape;

public class Line : Shape {
    public Vector2 Point1 { set; get; }
    public Vector2 Point2 { set; get; }

    public Line() : base() {
        Point1 = new(1.0f, 1.0f);
        Point2 = new(-1.0f, -1.0f);
    }

    public Line(float x1, float y1, float x2, float y2, Vector2 translate, float rotate, float scale, Vector3 color, bool fill, bool visible)
                    : base(translate, rotate, scale, color, fill, visible) {
        Point1 = new(x1, y1);
        Point2 = new(x2, y2);
    }

    public override Vector2[] GetLines() {
        return [Point1, Point2];
    }

    public override Vector2[] GetTriangles() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у линии
        */
        return [];
    }
}