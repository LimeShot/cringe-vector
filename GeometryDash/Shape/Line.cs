using OpenTK.Mathematics;

using System.Composition;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Line")]
[ExportMetadata("Icon", "Line.png")]

public class Line : IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public float Scale { set; get; }
    public ShapeStyle Style { set; get; }

    public Vector2 Point1 { set; get; }
    public Vector2 Point2 { set; get; }

    public Line() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Scale = 1.0f;
        Style = new ShapeStyle();
        Point1 = new(1.0f, 1.0f);
        Point2 = new(-1.0f, -1.0f);
    }

    public Line(float x1, float y1, float x2, float y2, Vector2 translate, float rotate, float scale, Vector3 colorOutline, Vector3 colorFill, bool fill, bool visible) {
        Translate = translate;
        Rotate = rotate;
        Scale = scale;
        Style = new ShapeStyle(colorOutline, colorFill, fill, visible);
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