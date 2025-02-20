using OpenTK.Mathematics;

using System.Composition;

namespace CringeCraft.GeometryDash.Shape;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Ellipse")]

public class Ellipse : IShape {
    public Vector2 Translate { set; get; }
    public float Rotate { set; get; }
    public float Scale { set; get; }
    public ShapeStyle Style { set; get; }
    public static string Icon => "Ellipse.png";
    public Vector2 Size { set; get; }

    public Ellipse() {
        Translate = (0.0f, 0.0f);
        Rotate = 0.0f;
        Scale = 1.0f;
        Style = new ShapeStyle();
        Size = new(1.0f, 1.0f);
    }

    public Ellipse(float x, float y, Vector2 translate, float rotate, float scale, Vector3 colorOutline, Vector3 colorFill, bool fill, bool visible) {
        Translate = translate;
        Rotate = rotate;
        Scale = scale;
        Style = new ShapeStyle(colorOutline, colorFill, fill, visible);
        Size = new(x, y);
    }

    public Vector2[] GetLineVertices() {
        /* TODO 
        Разобраться, что должен возвращать этот метод у элипса
        */
        return [];
    }

    public Vector2[] GetTriangleVertices() {
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
}