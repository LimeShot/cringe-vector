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
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        for (int i = 0; i < 4; i++) {
            BoundingBox[i] = result * Nodes[i] + Translate;
        }
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

    public Ellipse(Vector2 p1, float z, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            Nodes[i] = Vector2.Zero;
        }
        CalcBB();
    }

    public Ellipse(Vector2 p1, float diagonal, float z, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        float diagonalDev2 = diagonal / 2;
        Nodes[0] = new(-diagonalDev2, diagonalDev2);
        Nodes[1] = new(diagonalDev2, diagonalDev2);
        Nodes[2] = new(diagonalDev2, -diagonalDev2);
        Nodes[3] = new(-diagonalDev2, -diagonalDev2);
        CalcBB();
    }

    public float[] GetLineVertices() {
        return [];
    }

    public float[] GetTriangleVertices() {
        return [];
    }

    public float[] GetCircumferenceVertices() {
        float width = Math.Abs(Nodes[0].X - Nodes[2].X);
        float height = Math.Abs(Nodes[0].Y - Nodes[2].Y);
        return [Translate.X, Translate.Y, Z, width, height, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
    }

    public float[] GetCircleVertices() {
        float width = Math.Abs(Nodes[0].X - Nodes[2].X);
        float height = Math.Abs(Nodes[0].Y - Nodes[2].Y);
        return [Translate.X, Translate.Y, Z, width, height, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z];
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
        // TODO: Переделать под новый интерфейс

        /*float deltaX = x2 - x1;
        float deltaY = y2 - y1;

        Translate += new Vector2(deltaX, deltaY);*/
    }

    public void MoveNode(int index, Vector2 newNode) {

    }

    public void Resize(int index, Vector2 newNode) {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-Rotate), out Matrix2 result);
        Vector2 deltaDev2 = (newNode - BoundingBox[index]) / 2;
        Translate += deltaDev2;
        deltaDev2 = result * deltaDev2;
        Nodes[index] += deltaDev2;
        Nodes[(index + 2) % 4] -= deltaDev2;
        Nodes[(index + 1) % 4] = (Nodes[(index + 2) % 4].X, Nodes[index].Y);
        Nodes[(index + 3) % 4] = (Nodes[index].X, Nodes[(index + 2) % 4].Y);
        CalcBB();
        return;
    }

    public event Action? OnChange;
}