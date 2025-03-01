namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Ellipse")]
[ExportMetadata("Icon", "ellipse.png")]
public partial class Ellipse : ObservableObject, IShape {
    [ObservableProperty]
    private Vector2 _translate;

    [ObservableProperty]
    private float _rotate;

    [ObservableProperty]
    public ShapeStyle _style;

    [ObservableProperty]
    public Vector2 _size;

    public Ellipse() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Style = new();
        Size = new(1.0f, 1.0f);
    }

    public float[] GetLineVertices() {
        // TODO: Разобраться, что должен возвращать этот метод у элипса
        return [];
    }

    public float[] GetTriangleVertices() {
        // TODO: Разобраться, что должен возвращать этот метод у элипса
        return [];
    }

    public Vector2[] GetBoundingBox() {
        // TODO: Разобраться, что должен возвращать этот метод у элипса
        return [];
    }

    public bool ContainsPoint(Vector2 point) {
        float dx = point.X - Translate.X;
        float dy = point.Y - Translate.Y;

        float angle = -MathHelper.DegreesToRadians(Rotate);

        float xRot = dx * MathF.Cos(angle) - dy * MathF.Sin(angle);
        float yRot = dx * MathF.Sin(angle) + dy * MathF.Cos(angle);

        float a = Size.X / 2;
        float b = Size.Y / 2;

        return (xRot * xRot) / (a * a) + (yRot * yRot) / (b * b) <= 1;
    }

    public void Move(float x1, float y1, float x2, float y2) {
        float deltaX = x2 - x1;
        float deltaY = y2 - y1;

        Translate += new Vector2(deltaX, deltaY);
    }
}