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