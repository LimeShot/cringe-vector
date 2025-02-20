using OpenTK.Mathematics;

using System.Composition;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Rectangle")]

public class Rectangle : IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public float Scale { set; get; }
    public ShapeStyle Style { set; get; }
    public static string Icon => "Rectangle.png";
    public Vector2 Size { set; get; }

    public Rectangle() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Scale = 1.0f;
        Style = new ShapeStyle();
        Size = new(1.0f, 1.0f);
    }

    public Rectangle(float x, float y, Vector2 translate, float rotate, float scale, Vector3 colorOutline, Vector3 colorFill, bool fill, bool visible) {
        Translate = translate;
        Rotate = rotate;
        Scale = scale;
        Style = new ShapeStyle(colorOutline, colorFill, fill, visible);
        Size = new(x, y);
    }

    public Vector2[] GetLineVertices() {
        return [new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2),
                new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2),

                new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2),

                new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2),

                new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2),
                new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2)];
    }

    public Vector2[] GetTriangleVertices() {
        return [new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2),
                new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2),

                new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2)];
    }
    public Vector2[] GetBoundingBox() {
        return [new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2),
                new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2),

                new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2),

                new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2),
                new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2),

                new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2),
                new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2)];
    }
}