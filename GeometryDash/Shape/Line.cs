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
        BoundingBox[0] = result * Nodes[0] + Translate;
        BoundingBox[1] = result * new Vector2(Nodes[0].X, Nodes[1].Y) + Translate;
        BoundingBox[2] = result * Nodes[1] + Translate;
        BoundingBox[3] = result * new Vector2(Nodes[1].X, Nodes[0].Y) + Translate;
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

    public float[] GetLineVertices() {
        // TODO: Необходимо возвращать [x,y,Z,color.x,color.y,,color.z] 2 вершины
        return [];
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

    public bool IsBelongsShape(Vector2 point) {
        // TODO: Переделать под новый интерфейс

        /*Vector2 localPoint = point - Translate;

        float angle = -MathHelper.DegreesToRadians(Rotate);

        float xRot = localPoint.X * MathF.Cos(angle) - localPoint.Y * MathF.Sin(angle);
        float yRot = localPoint.X * MathF.Sin(angle) + localPoint.Y * MathF.Cos(angle);
        localPoint = new(xRot, yRot);

        Vector2 localP1 = Point1 - Translate;
        Vector2 localP2 = Point2 - Translate;

        Vector2 lineVec = localP2 - localP1;
        Vector2 pointVec = localPoint - localP1;

        float projection = Vector2.Dot(pointVec, lineVec) / lineVec.LengthSquared;
        if (projection < 0 || projection > 1) return false;

        Vector2 closestPoint = localP1 + projection * lineVec;
        return (localPoint - closestPoint).Length < 0.01f;*/
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

        Translate += new Vector2(deltaX, deltaY);

        Point1 += new Vector2(deltaX, deltaY);
        Point2 += new Vector2(deltaX, deltaY);*/
    }

    public void MoveNode(int index, Vector2 newNode) {

    }

    public void Resize(int index, Vector2 newNode) {
        // TODO: Реализовать метод
        switch (index) {
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            default:

                break;
        }
        return;
    }

    public event Action? OnChange;
}