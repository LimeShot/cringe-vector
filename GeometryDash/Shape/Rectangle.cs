namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Rectangle")]
[ExportMetadata("Icon", "rectangle.png")]
public partial class Rectangle : IShape {
    public Vector2 Translate { get; }
    public float Z { set; get; }
    public float Rotate { get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { get; }
    public Vector2[] Nodes { set; get; }

    public Rectangle() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
    }

    public float[] GetLineVertices() {
        // TODO: Необходимо возвращать [x,y,Z,color.x,color.y,,color.z]
        return [];
    }

    public float[] GetTriangleVertices() {
        // TODO: Необходимо возвращать [x,y,Z,color.x,color.y,,color.z]
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

        float halfWidth = Size.X / 2;
        float halfHeight = Size.Y / 2;

        return xRot >= -halfWidth && xRot <= halfWidth &&
            yRot >= -halfHeight && yRot <= halfHeight;*/
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