namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

public class ShapeStyle {
    public Vector3 ColorOutline { set; get; }
    public Vector3 ColorFill { set; get; }
    public bool Fill { set; get; }
    public bool Visible { set; get; }

    public ShapeStyle() {
        ColorOutline = (0.0f, 0.0f, 0.0f);
        ColorFill = (0.0f, 0.0f, 0.0f);
        Fill = false;
        Visible = true;
    }

    public ShapeStyle(Vector3 colorOutline, Vector3 colorFill, bool fill, bool visible) {
        ColorOutline = colorOutline;
        ColorFill = colorFill;
        Fill = fill;
        Visible = visible;
    }

    public ShapeStyle(ShapeStyle other) {
        ColorOutline = other.ColorOutline;
        ColorFill = other.ColorFill;
        Fill = other.Fill;
        Visible = other.Visible;
    }

    public ShapeStyle Clone() {
        return new ShapeStyle(ColorOutline, ColorFill, Fill, Visible);
    }

    public static bool operator ==(ShapeStyle a, ShapeStyle b) {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.ColorOutline == b.ColorOutline
            && a.ColorFill == b.ColorFill
            && a.Fill == b.Fill
            && a.Visible == b.Visible;
    }

    public static bool operator !=(ShapeStyle a, ShapeStyle b) => !(a == b);

    public void CopyFrom(ShapeStyle other) {
        this.ColorOutline = other.ColorOutline;
        this.ColorFill = other.ColorFill;
        this.Fill = other.Fill;
        this.Visible = other.Visible;
    }
}