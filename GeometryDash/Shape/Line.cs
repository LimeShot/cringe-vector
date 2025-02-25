using OpenTK.Mathematics;

using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Line")]
[ExportMetadata("Icon", "Line.png")]

public partial class Line : ObservableObject, IShape {
    
    [ObservableProperty]
    private Vector2 _translate;

    [ObservableProperty]
    private float _rotate;

    [ObservableProperty]
    public ShapeStyle _style;

    [ObservableProperty]
    private Vector2 _point1;

    [ObservableProperty]
    private Vector2 _point2;

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
    public bool ContainsPoint(Vector2 point) {

        Vector2 localPoint = point - Translate;

        float angle = -MathHelper.DegreesToRadians(Rotate);

        float xRot = localPoint.X * MathF.Cos(angle) - localPoint.Y * MathF.Sin(angle);
        float yRot = localPoint.X * MathF.Sin(angle) + localPoint.Y * MathF.Cos(angle);
        localPoint = new Vector2(xRot, yRot);

        Vector2 localP1 = Point1 - Translate;
        Vector2 localP2 = Point2 - Translate;

        Vector2 lineVec = localP2 - localP1;
        Vector2 pointVec = localPoint - localP1;

        float projection = Vector2.Dot(pointVec, lineVec) / lineVec.LengthSquared;
        if (projection < 0 || projection > 1) return false;

        Vector2 closestPoint = localP1 + projection * lineVec;
        return (localPoint - closestPoint).Length < 0.01f;
    }
}