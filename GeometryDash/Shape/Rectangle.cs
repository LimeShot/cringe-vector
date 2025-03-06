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
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        for (int i = 0; i < 4; i++) {
            BoundingBox[i] = result * Nodes[i] + Translate;
        }
    }

    public Rectangle() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            Nodes[i] = Vector2.Zero;
        }
        CalcBB();
    }

    public Rectangle(Vector2 p1, float z, ShapeStyle? shapeStyle = null) {
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

    public Rectangle(Vector2 p1, float side, float z, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        float sideDev2 = side / 2;
        Nodes[0] = new(-sideDev2, sideDev2);
        Nodes[1] = new(sideDev2, sideDev2);
        Nodes[2] = new(sideDev2, -sideDev2);
        Nodes[3] = new(-sideDev2, -sideDev2);
        CalcBB();
    }

    public float[] GetLineVertices() {
        float[] args = [Z, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
        return [Nodes[0].X , Nodes[0].Y , ..args,
                Nodes[1].X , Nodes[1].Y , ..args,

                Nodes[1].X , Nodes[1].Y , ..args,
                Nodes[2].X , Nodes[2].Y , ..args,

                Nodes[2].X , Nodes[2].Y , ..args,
                Nodes[3].X , Nodes[3].Y , ..args,

                Nodes[3].X , Nodes[3].Y , ..args,
                Nodes[0].X , Nodes[0].Y , ..args];
    }

    public float[] GetTriangleVertices() {
        if (!Style.Fill) return [];
        float[] args = [Z, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
        return [Nodes[0].X , Nodes[0].Y , ..args,
                Nodes[1].X , Nodes[1].Y , ..args,
                Nodes[2].X , Nodes[2].Y , ..args,

                Nodes[0].X , Nodes[0].Y , ..args,
                Nodes[2].X , Nodes[2].Y , ..args,
                Nodes[3].X , Nodes[3].Y , ..args];
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
        int diagIndex = (index + 2) % 4;
        Nodes[index] += deltaDev2;
        Nodes[diagIndex] -= deltaDev2;
        if (index % 2 == 0) {
            Nodes[(index + 1) % 4] = (Nodes[diagIndex].X, Nodes[index].Y);
            Nodes[(index + 3) % 4] = (Nodes[index].X, Nodes[diagIndex].Y);
        } else {
            Nodes[(index + 1) % 4] = (Nodes[index].X, Nodes[diagIndex].Y);
            Nodes[(index + 3) % 4] = (Nodes[diagIndex].X, Nodes[index].Y);
        }
        if (Nodes[0].X - Nodes[1].X > 0.0f && MathF.Abs(Nodes[0].X - Nodes[1].X) > 1E-6) {
            (Nodes[0], Nodes[1]) = (Nodes[1], Nodes[0]);
            (Nodes[3], Nodes[2]) = (Nodes[2], Nodes[3]);
        }
        if (Nodes[0].Y - Nodes[3].Y < 0.0f && MathF.Abs(Nodes[0].Y - Nodes[3].Y) > 1E-6) {
            (Nodes[1], Nodes[2]) = (Nodes[2], Nodes[1]);
            (Nodes[0], Nodes[3]) = (Nodes[3], Nodes[0]);
        }
        CalcBB();
    }

    public event Action? OnChange;
}