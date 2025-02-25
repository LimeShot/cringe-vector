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
}