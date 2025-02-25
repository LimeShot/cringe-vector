using OpenTK.Mathematics;

using System.Composition;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Rectangle")]
[ExportMetadata("Icon", "Rectangle.png")]

public class Rectangle : ObservableObject, IShape {
    
    [ObservableProperty]
    private Vector2 _translate;

    [ObservableProperty]
    private float _rotate;

    [ObservableProperty]
    public ShapeStyle _style;

    [ObservableProperty]
    public Vector2 _size;

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

}