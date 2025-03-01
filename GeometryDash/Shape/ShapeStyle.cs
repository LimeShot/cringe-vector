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
        Fill = true;
        Visible = true;
    }

    public ShapeStyle(Vector3 colorOutline, Vector3 colorFill, bool fill, bool visible) {
        ColorOutline = colorOutline;
        ColorFill = colorFill;
        Fill = fill;
        Visible = visible;
    }
}