using OpenTK.Mathematics;

using System.Composition;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Ellipse")]
[ExportMetadata("Icon", "Ellipse.png")]

public class Ellipse : IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2 Size { set; get; }

    public Ellipse() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Style = new ShapeStyle();
        Size = new(1.0f, 1.0f);
    }

    public float[] GetLineVertices() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у элипса
        */
        return [];
    }

    public float[] GetTriangleVertices() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у элипса
        */
        return [];
    }
    public Vector2[] GetBoundingBox() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у элипса
        */
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
}