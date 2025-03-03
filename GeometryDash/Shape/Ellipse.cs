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

    public bool IsBelongsShape(Vector2 point) {
        // TODO: Переделать под новый интерфейс

        /*float dx = point.X - Translate.X;
        float dy = point.Y - Translate.Y;

        float angle = -MathHelper.DegreesToRadians(Rotate);

        float xRot = dx * MathF.Cos(angle) - dy * MathF.Sin(angle);
        float yRot = dx * MathF.Sin(angle) + dy * MathF.Cos(angle);

        float a = Size.X / 2;
        float b = Size.Y / 2;

        return (xRot * xRot) / (a * a) + (yRot * yRot) / (b * b) <= 1;*/
        return false;
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