namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Ellipse")]
[ExportMetadata("Icon", "ellipse.png")]
public partial class Ellipse : IShape {
    // TODO: Добавить event OnChange в методы set
    public Vector2 Translate { private set; get; }
    public float Z { set; get; }
    public float Rotate { private set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { private set; get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        // TODO: Посчитать в абсолютных координатах, с учетом Rotate и Translate, и запихнуть в BoundingBox
        return;
    }

    public Ellipse() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        CalcBB();
    }

    public float[] GetLineVertices() {
        return [];
    }

    public float[] GetTriangleVertices() {
        return [];
    }

    public float[] GetCircumferenceVertices() {
        // TODO: [Translate.X,Translate.Y,Z,width,height,ColorOutline.X,ColorOutline.Y,ColorOutline.Z]
        return [];
    }

    public float[] GetCircleVertices() {
        // TODO: [Translate.X,Translate.Y,Z,width,height,ColorFill.X,ColorFill.Y,ColorFill.Z]
        return [];
    }

    public bool IsBelongsShape(Vector2 point, float radiusPoint) {
        Vector2 localPoint = point - Translate;

        float angle = -MathHelper.DegreesToRadians(Rotate);
        float xRot = localPoint.X * MathF.Cos(angle) - localPoint.Y * MathF.Sin(angle);
        float yRot = localPoint.X * MathF.Sin(angle) + localPoint.Y * MathF.Cos(angle);

        Vector2 localP1 = Nodes[0] - Translate;
        Vector2 localP2 = Nodes[1] - Translate;

        float a = Math.Abs(localP2.X - localP1.X) / 2 + radiusPoint;
        float b = Math.Abs(localP2.Y - localP1.Y) / 2 + radiusPoint;

        return (xRot * xRot) / (a * a) + (yRot * yRot) / (b * b) <= 1;
    }


    public int IsBBNode(Vector2 point) {
        // TODO: Реализовать метод
        return 0;
    }

    public void Move(Vector2 oldPoint, Vector2 newPoint) {
        // TODO: Переделать под новый интерфейс + добавить вызов ивента OnChange

        /*float deltaX = x2 - x1;
        float deltaY = y2 - y1;

        Translate += new Vector2(deltaX, deltaY);*/
    }

    public void MoveNode(int index, Vector2 newNode) {

    }

    public void Resize(int index, Vector2 newNode) {
        // TODO: Реализовать метод
        return;
    }

    public event Action? OnChange;
}