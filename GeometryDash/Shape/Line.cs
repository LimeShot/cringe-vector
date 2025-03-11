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
    public Vector2[] LocalBoundingBox { private set; get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        for (int i = 0; i < 2; i++) {
            LocalBoundingBox[i] = result * Nodes[i];
            BoundingBox[i] = LocalBoundingBox[i] + Translate;
        }
    }

    public Line() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[2];
        LocalBoundingBox = new Vector2[2];
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
        BoundingBox = new Vector2[2];
        LocalBoundingBox = new Vector2[2];
        Nodes = new Vector2[2];
        float lengthDev2 = length / 2;
        Nodes[0] = new(-lengthDev2, 0.0f);
        Nodes[1] = new(lengthDev2, 0.0f);
        CalcBB();
    }

    public float[] GetLineVertices() {
        float[] args = [Z, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
        return [Nodes[0].X, Nodes[0].Y, ..args,
                Nodes[1].X, Nodes[1].Y, ..args];
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

        Vector2 localP1 = Nodes[0];
        Vector2 localP2 = Nodes[1];

        Vector2 lineVector = localP2 - localP1;
        float lineLength = lineVector.Length;

        Vector2 lineDir = lineVector.Normalized();

        Vector2 pointVec = localPoint - localP1;

        float projection = Vector2.Dot(pointVec, lineDir);

        bool withinLength = projection >= -radiusPoint && projection <= lineLength + radiusPoint;

        Vector2 closestPointOnLine = localP1 + lineDir * Math.Clamp(projection, -radiusPoint, lineLength + radiusPoint);

        float distanceToLine = (localPoint - closestPointOnLine).Length;

        bool withinWidth = distanceToLine <= radiusPoint + 3.0f;

        return withinLength && withinWidth;
    }

    public int IsBBNode(Vector2 point) {
        // TODO: Реализовать метод
        return 0;
    }

    public void Move(Vector2 delta) {
        // TODO: Переделать под новый интерфейс
        Translate += delta;
        CalcBB();
    }

    public void MoveNode(int index, Vector2 newNode) {

    }

    public void Resize(int index, Vector2 newNode) {
        Vector2 deltaDev2 = (newNode - BoundingBox[index]) / 2;
        Vector2 RotateNodes = BoundingBox[index] - Translate + deltaDev2;
        Translate += deltaDev2;
        double denominator = RotateNodes.Length;
        if (Math.Abs(denominator) < 1E-10)
            denominator = 1E-7;
        float angle = (float)MathHelper.RadiansToDegrees(Math.Acos(RotateNodes.X / denominator));
        int sign = 1;
        if (-RotateNodes.Y < 0)
            sign = -1;
        Rotate = sign * angle;
        Nodes[0].X = -(float)denominator;
        Nodes[1].X = (float)denominator;
        CalcBB();
    }

    public string ShapeType => GetType().Name;
    public string IconPath => $"pack://siteoforigin:,,,/assets/tools/{ShapeType.ToLower()}.png";

    public void RotateShape(Vector2 p1, Vector2 p2) {
        p1 -= Translate;
        p2 -= Translate;
        double denominator = p1.Length * p2.Length;
        if (Math.Abs(denominator) < 1E-10)
            denominator = 1E-7;
        float angle = (float)MathHelper.RadiansToDegrees(Math.Acos((p1.X * p2.X + p1.Y * p2.Y) / denominator));
        int sign = 1;
        if (new Matrix2(p1, p2).Determinant < 0)
            sign = -1;
        Rotate += sign * angle;
        Rotate %= 360;
        CalcBB();
    }

    public void NormalizeIndexNodes() {
        if (Nodes[0].X > 0.0f) {
            (Nodes[0], Nodes[1]) = (Nodes[1], Nodes[0]);
            CalcBB();
        }
    }

    public void Reflect() {
        // TODO: Реализовать метод
    }
}