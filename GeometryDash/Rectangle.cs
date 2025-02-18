using OpenTK.Mathematics;

using GeometryDash.Shape;

namespace GeometryDash.Shape.Rectangle;

public class Rectangle : Shape {
    public Vector2 Size { set; get; }

    public Rectangle() : base() {
        Size = new(1.0f, 1.0f);
    }

    public Rectangle(float x, float y, Vector2 translate, float rotate, float scale, Vector3 color, bool fill, bool visible)
                    : base(translate, rotate, scale, color, fill, visible) {
        Size = new(x, y);
    }

    public override Vector2[] GetLines() {
        if (Style.Fill)
            return [];
        Vector2[] arr = [new()];
        arr.Append(new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2));
        arr.Append(new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2));

        arr.Append(new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2));
        arr.Append(new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2));

        arr.Append(new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2));
        arr.Append(new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2));

        arr.Append(new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2));
        arr.Append(new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2));

        return arr;
    }

    public override Vector2[] GetTriangles() {
        if (!Style.Fill)
            return [];
        Vector2[] arr = [new()];
        arr.Append(new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2));
        arr.Append(new(Translate.X + Size.X / 2, Translate.Y - Size.Y / 2));
        arr.Append(new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2));

        arr.Append(new(Translate.X + Size.X / 2, Translate.Y + Size.Y / 2));
        arr.Append(new(Translate.X - Size.X / 2, Translate.Y - Size.Y / 2));
        arr.Append(new(Translate.X - Size.X / 2, Translate.Y + Size.Y / 2));
        return arr;
    }
}