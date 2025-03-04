namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Line")]
[ExportMetadata("Icon", "line.png")]
public partial class Line : IShape {
    // TODO: Добавить event OnChange в методы set
    public Vector2 Translate { private set; get; }
    public float Z { set; get; }
    public float Rotate { private set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { private set; get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        BoundingBox[0] = result * new Vector2(Nodes[0].X, Nodes[1].Y) + Translate;
        BoundingBox[1] = result * Nodes[1] + Translate;
        BoundingBox[2] = result * new Vector2(Nodes[1].X, Nodes[0].Y) + Translate;
        BoundingBox[3] = result * Nodes[0] + Translate;
    }

    public Line() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[2];
        Nodes[0] = Vector2.Zero;
        Nodes[1] = Vector2.Zero;
        CalcBB();
    }

    public Line(Vector2 p1, float z, ShapeStyle? shapeStyle = null) : this() {
        Translate = p1;
        Z = z;
        Style = shapeStyle ?? new();
        CalcBB();
    }

    public Line(Vector2 p1, float length, float z, ShapeStyle? shapeStyle = null) : this() {
        Translate = p1;
        Z = z;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[2];
        float lengthDev2 = length / 2;
        Nodes[0] = new(-lengthDev2, 0.0f);
        Nodes[1] = new(lengthDev2, 0.0f);
        CalcBB();
    }

    public float[] GetLineVertices() {
        return [Nodes[0].X, Nodes[0].Y, Z, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z,
                Nodes[1].X, Nodes[1].Y, Z, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
    }

    public float[] GetTriangleVertices() {
        return [];
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

        Vector2 localP1 = Nodes[0] - Translate;
        Vector2 localP2 = Nodes[1] - Translate;
        Vector2 lineVector = localP2 - localP1;
        float lineLength = lineVector.Length;

        Vector2 lineDir = lineVector.Normalized();

        Vector2 pointVec = localPoint - localP1;
        float projection = Vector2.Dot(pointVec, lineDir);

        bool withinLength = projection >= -radiusPoint && projection <= lineLength + radiusPoint;

        Vector2 normal = new Vector2(-lineDir.Y, lineDir.X);
        float distanceToLine = Math.Abs(Vector2.Dot(normal, pointVec));

        bool withinWidth = distanceToLine <= radiusPoint;

        return withinLength && withinWidth;
    }


    public int IsBBNode(Vector2 point) {
        // TODO: Реализовать метод
        return 0;
    }

    public void Move(Vector2 oldPoint, Vector2 newPoint) {
        // TODO: Переделать под новый интерфейс

        /*float deltaX = x2 - x1;
        float deltaY = y2 - y1;

        Translate += new Vector2(deltaX, deltaY);

        Point1 += new Vector2(deltaX, deltaY);
        Point2 += new Vector2(deltaX, deltaY);*/
    }

    public void MoveNode(int index, Vector2 newNode) {

    }

    public void Resize(int index, Vector2 newNode) {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-Rotate), out Matrix2 result);
        Vector2 deltaDev2 = (newNode - BoundingBox[index]) / 2;
        Translate += deltaDev2;
        deltaDev2 = result * deltaDev2;
        Nodes[index] += deltaDev2;
        Nodes[(index + 1) % 2] -= deltaDev2;
        CalcBB();
    }

    public event Action? OnChange;
}