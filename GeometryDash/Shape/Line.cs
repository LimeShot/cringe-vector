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

    public float[] GetLineVertices() {
        return [Point1.X, Point1.Y, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Point2.X, Point2.Y, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
    }

    public float[] GetTriangleVertices() {
        return [];
    }
    public Vector2[] GetBoundingBox() {
        return [new(Point1.X, Point1.Y),
                new(Point2.X, Point2.Y)];
    }
}