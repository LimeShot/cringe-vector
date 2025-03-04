namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Rectangle")]
[ExportMetadata("Icon", "rectangle.png")]
public partial class Rectangle : IShape {
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

    public Rectangle() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        CalcBB();
    }

    public float[] GetLineVertices() {
        return [Nodes[0].X / 2, Nodes[0].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Nodes[1].X / 2, Nodes[1].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,

                Nodes[1].X / 2, Nodes[1].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Nodes[2].X / 2, Nodes[2].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,

                Nodes[2].X / 2, Nodes[2].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Nodes[3].X / 2, Nodes[3].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,

                Nodes[3].X / 2, Nodes[3].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Nodes[0].X / 2, Nodes[0].Y / 2, Z, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
    }

    public float[] GetTriangleVertices() {
        return [Nodes[0].X / 2, Nodes[0].Y / 2, Z, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                Nodes[1].X / 2, Nodes[1].Y / 2, Z, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                Nodes[2].X / 2, Nodes[2].Y / 2, Z, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,

                Nodes[0].X / 2, Nodes[0].Y / 2, Z, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                Nodes[2].X / 2, Nodes[2].Y / 2, Z, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z,
                Nodes[3].X / 2, Nodes[3].Y / 2, Z, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z];
    }

    public float[] GetCircumferenceVertices() {
        return [];
    }

    public float[] GetCircleVertices() {
        return [];
    }

    public bool IsBelongsShape(Vector2 point, float radiusPoint) {
        Vector2 localPoint = point - Translate;

        float angle = -MathHelper.DegreesToRadians(Rotate);
        float xRot = localPoint.X * MathF.Cos(angle) - localPoint.Y * MathF.Sin(angle);
        float yRot = localPoint.X * MathF.Sin(angle) + localPoint.Y * MathF.Cos(angle);
        localPoint = new Vector2(xRot, yRot);

        float halfWidth = (Nodes[1].X - Nodes[0].X) / 2 + radiusPoint;
        float halfHeight = (Nodes[1].Y - Nodes[0].Y) / 2 + radiusPoint;

        return xRot >= -halfWidth && xRot <= halfWidth &&
               yRot >= -halfHeight && yRot <= halfHeight;
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