namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Line")]
[ExportMetadata("Icon", "line.png")]
public partial class Line : IShape {
    public Vector2 Translate { get; }
    public float Z { set; get; }
    public float Rotate { get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        // TODO: Посчитать в абсолютных координатах, с учетом Rotate и Translate, и запихнуть в BoundingBox
        return;
    }

    public Line() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[2];
    }

    public Line(Vector2 p1, Vector2 p2, float z, ShapeStyle? shapeStyle = null) {
        // TODO: Доделать конструктор по двум точкам
        Translate = ((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        //Надо проверить, не будет ли засорятся память
        Style = shapeStyle ?? new();
        Z = z;
        float lenDev2 = (p2 - p1).Length / 2;
        if (p1.X > p2.X) {
            Rotate = (float)MathHelper.Acos(lenDev2 / (p1.X - Translate.X));
            Nodes[0] = (lenDev2, 0.0f);
            Nodes[1] = (-lenDev2, 0.0f);
        } else {
            Rotate = (float)MathHelper.Acos(lenDev2 / (p2.X - Translate.X));
            Nodes[0] = (-lenDev2, 0.0f);
            Nodes[1] = (lenDev2, 0.0f);
        }
        CalcBB();
    }

    public float[] GetLineVertices() {
        // TODO: Необходимо возвращать [x,y,Z,color.x,color.y,,color.z]
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
        // TODO: Реализовать метод
        return;
    }

    public event Action? OnChange;
}