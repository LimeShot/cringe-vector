using OpenTK.Mathematics;

using System.Composition;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Rectangle")]
[ExportMetadata("Icon", "Rectangle.png")]

public class Rectangle : IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2 Size { set; get; }

    public Rectangle() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Style = new ShapeStyle();
        Size = new(1.0f, 1.0f);
    }


    public float[] GetLineVertices() {
        return [Size.X / 2, Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Size.X / 2, -Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,

                Size.X / 2, -Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                -Size.X / 2, -Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,

                -Size.X / 2, -Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                -Size.X / 2, Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,

                -Size.X / 2, Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Size.X / 2, Size.Y / 2, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
    }

    public float[] GetTriangleVertices() {
        return [Size.X / 2, Size.Y / 2, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                Size.X / 2, -Size.Y / 2, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                -Size.X / 2, -Size.Y / 2, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,

                Size.X / 2, Size.Y / 2, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                -Size.X / 2, -Size.Y / 2, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                -Size.X / 2, Size.Y / 2, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z];
    }
    public Vector2[] GetBoundingBox() {
        return [new(Size.X / 2, Size.Y / 2),
                new(Size.X / 2, -Size.Y / 2),

                new(Size.X / 2, -Size.Y / 2),
                new(-Size.X / 2, -Size.Y / 2),

                new(-Size.X / 2, Size.Y / 2),
                new(-Size.X / 2, Size.Y / 2),

                new(-Size.X / 2, Size.Y / 2),
                new(Size.X / 2, Size.Y / 2)];
    }
    public bool ContainsPoint(Vector2 point) {

        Vector2 localPoint = point - Translate;

        float angle = -MathHelper.DegreesToRadians(Rotate);

        float xRot = localPoint.X * MathF.Cos(angle) - localPoint.Y * MathF.Sin(angle);
        float yRot = localPoint.X * MathF.Sin(angle) + localPoint.Y * MathF.Cos(angle);

        float halfWidth = Size.X / 2;
        float halfHeight = Size.Y / 2;

        return (xRot >= -halfWidth && xRot <= halfWidth &&
                yRot >= -halfHeight && yRot <= halfHeight);
    }
    public void Move(float x1, float y1, float x2, float y2) {
        float deltaX = x2 - x1;
        float deltaY = y2 - y1;

        Translate += new Vector2(deltaX, deltaY);
    }

}