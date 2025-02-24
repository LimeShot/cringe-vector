using OpenTK.Mathematics;

using System.Composition;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Line")]
[ExportMetadata("Icon", "Line.png")]

public class Line : IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public ShapeStyle Style { set; get; }

    public Vector2 Point1 { set; get; }
    public Vector2 Point2 { set; get; }

    public Line(float x1, float y1, float x2, float y2, ShapeStyle? shapeStyle = null) {
        Translate = ((x1 + x2) / 2, (y1 + y2) / 2);
        Rotate = 0.0f;
        //Надо проверить, не будет ли засорятся память
        Style = shapeStyle ?? new ShapeStyle();
        Point1 = new(x1, y1);
        Point2 = new(x2, y2);
    }

    public Vector2[] GetLineVertices() {
        return [new(Translate.X + Point1.X, Translate.Y + Point1.Y),
                new(Translate.X + Point2.X, Translate.Y + Point2.Y)];
    }

    public Vector2[] GetTriangleVertices() {
        return [];
    }
    public Vector2[] GetBoundingBox() {
        return [new(Translate.X + Point1.X, Translate.Y + Point1.Y),
                new(Translate.X + Point2.X, Translate.Y + Point2.Y)];
    }
}