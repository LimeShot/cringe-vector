namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;
using EarClipperLib;

using System.Composition;
using System.Diagnostics;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Polygon")]
[ExportMetadata("Icon", "polygon.png")]
public partial class Polygon : IShape, IChangableShape {
    // TODO: Добавить event OnChange в методы set
    public Vector2 Translate { private set; get; }
    public float Z { set; get; }
    public float DeltaZ { set; get; } // Для отрисовки контура на слой выше, чем заливки
    public float Rotate { private set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { private set; get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        Rotate = (float)Math.Round(Rotate, 1);
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        Vector2 maxPoint = new(0, 0), minPoint = new(0, 0);
        foreach (var node in Nodes) {
            maxPoint.X = MathF.Max(maxPoint.X, node.X);
            minPoint.X = MathF.Min(minPoint.X, node.X);
            maxPoint.Y = MathF.Max(maxPoint.Y, node.Y);
            minPoint.Y = MathF.Min(minPoint.Y, node.Y);
        }
        Vector2[] localBB =
        [
            result * new Vector2(minPoint.X, maxPoint.Y),
            result * maxPoint,
            result * new Vector2(maxPoint.X, minPoint.Y),
            result * minPoint,
        ];
        int curZero = 0;
        for (int i = 0; i < 4; i++) {
            if (localBB[i].X <= 0 && localBB[i].Y >= 0) {
                curZero = i;
                break;
            }
        }
        for (int i = 0; i < 4; i++) {
            BoundingBox[i] = localBB[(curZero + i) % 4] + Translate;
        }
    }

    public Polygon() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        DeltaZ = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        int countVertices = 6;
        Nodes = new Vector2[countVertices];
        for (int i = 0; i < countVertices; i++) {
            Nodes[i] = Vector2.Zero;
        }
        CalcBB();
    }

    public Polygon(Polygon other) {
        Translate = other.Translate;
        Z = other.Z;
        DeltaZ = other.DeltaZ;
        Rotate = other.Rotate;
        Style = other.Style.Clone();
        BoundingBox = new Vector2[other.BoundingBox.Length];
        Array.Copy(other.BoundingBox, this.BoundingBox, other.BoundingBox.Length);
        Nodes = new Vector2[other.Nodes.Length];
        Array.Copy(other.Nodes, this.Nodes, other.Nodes.Length);
    }

    public IShape Clone() {
        return new Polygon(this);
    }

    public Polygon(Vector2 p1, float z, float deltaZ, ShapeStyle? shapeStyle = null) {
        float diagonal = 1e-5f;
        Translate = p1;
        Z = z;
        DeltaZ = deltaZ;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        int countVertices = 6;
        Nodes = new Vector2[countVertices];
        float angle = -360.0f / countVertices;
        for (int i = 0; i < countVertices; i++) {
            Matrix2.CreateRotation(MathHelper.DegreesToRadians(i * angle), out Matrix2 result);
            Nodes[i] = result * new Vector2(diagonal, 0.0f);
        }
        CalcBB();
    }

    public Polygon(Vector2 p1, float diagonal, float z, float deltaZ, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
        DeltaZ = deltaZ;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        int countVertices = 6;
        Nodes = new Vector2[countVertices];
        float angle = -360.0f / countVertices;
        for (int i = 0; i < countVertices; i++) {
            Matrix2.CreateRotation(MathHelper.DegreesToRadians(i * angle), out Matrix2 result);
            Nodes[i] = result * new Vector2(diagonal / 2, 0.0f);
        }
        CalcBB();
    }

    public Polygon(Vector2 p1, float diagonal, int countVertices, float z, float deltaZ, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
        DeltaZ = deltaZ;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[countVertices];
        float angle = -360.0f / countVertices;
        for (int i = 0; i < countVertices; i++) {
            Matrix2.CreateRotation(MathHelper.DegreesToRadians(i * angle), out Matrix2 result);
            Nodes[i] = result * new Vector2(diagonal / 2, 0.0f);
        }
        CalcBB();
    }

    public float[] GetLineVertices() {
        if (!Style.Visible) return [];
        // TODO: Привести к нормальному виду
        int len = Nodes.Length;
        float[] returns = new float[len * 2 * 9];
        for (int i = 0; i < len; i++) {
            returns[i * 18] = Nodes[i].X;
            returns[i * 18 + 1] = Nodes[i].Y;
            returns[i * 18 + 2] = Z + DeltaZ;
            returns[i * 18 + 3] = Translate.X;
            returns[i * 18 + 4] = Translate.Y;
            returns[i * 18 + 5] = Rotate;
            returns[i * 18 + 6] = Style.ColorOutline.X;
            returns[i * 18 + 7] = Style.ColorOutline.Y;
            returns[i * 18 + 8] = Style.ColorOutline.Z;

            returns[i * 18 + 9] = Nodes[(i + 1) % len].X;
            returns[i * 18 + 10] = Nodes[(i + 1) % len].Y;
            returns[i * 18 + 11] = Z + DeltaZ;
            returns[i * 18 + 12] = Translate.X;
            returns[i * 18 + 13] = Translate.Y;
            returns[i * 18 + 14] = Rotate;
            returns[i * 18 + 15] = Style.ColorOutline.X;
            returns[i * 18 + 16] = Style.ColorOutline.Y;
            returns[i * 18 + 17] = Style.ColorOutline.Z;
        }
        return returns;
    }

    public float[] GetTriangleVertices() {
        if (!Style.Visible || !Style.Fill) return [];
        List<Vector3m> points = new List<Vector3m>();
        for (int i = 0; i < Nodes.Length; i++) {
            points.Add(new Vector3m(Nodes[i].X, Nodes[i].Y, 0));
        }
        EarClipping earClipping = new EarClipping();
        earClipping.SetPoints(points);
        earClipping.Triangulate();
        var res = earClipping.Result;
        int len = res.Count;
        float[] returns = new float[len * 9];
        for (int i = 0; i < len; i++) {
            returns[i * 9] = (float)res[i].X;
            returns[i * 9 + 1] = (float)res[i].Y;
            returns[i * 9 + 2] = Z;
            returns[i * 9 + 3] = Translate.X;
            returns[i * 9 + 4] = Translate.Y;
            returns[i * 9 + 5] = Rotate;
            returns[i * 9 + 6] = Style.ColorFill.X;
            returns[i * 9 + 7] = Style.ColorFill.Y;
            returns[i * 9 + 8] = Style.ColorFill.Z;
        }
        return returns;
    }

    public float[] GetCircumferenceVertices() {
        return [];
    }

    public float[] GetCircleVertices() {
        return [];
    }

    public bool IsBelongsShape(Vector2 point, float radiusPoint) {

        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-Rotate), out Matrix2 result);
        Vector2 localPoint = result * (point - Translate);

        // Ray Casting 
        int countIntersections = 0;
        int n = Nodes.Length;

        for (int i = 0; i < n; i++) {
            Vector2 p1 = Nodes[i];
            Vector2 p2 = Nodes[(i + 1) % n];
            if ((p1.Y > localPoint.Y) != (p2.Y > localPoint.Y)) {
                float xIntersect = (localPoint.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                if (localPoint.X < xIntersect) {
                    countIntersections++;
                }
            }
            Vector2 segment = p2 - p1;
            Vector2 pointToP1 = localPoint - p1;

            float lenSquared = segment.LengthSquared;
            if (lenSquared == 0) { // p1 и p2 совпадают
                if ((localPoint - p1).Length <= radiusPoint) return true;
                continue;
            }
            float t = MathF.Max(0, MathF.Min(1, Vector2.Dot(pointToP1, segment) / lenSquared));
            Vector2 projection = p1 + t * segment;

            if ((localPoint - projection).Length <= radiusPoint) {
                return true;
            }
        }
        return countIntersections % 2 == 1;
    }

    public int IsBBNode(Vector2 point) {
        // TODO: Реализовать метод
        return 0;
    }

    public void Move(Vector2 delta) {
        Translate += delta;
        CalcBB();
    }

    public void MoveNode(int index, Vector2 newNode) {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-Rotate), out Matrix2 result);
        Vector2 notRotateNewNode = result * (newNode - Translate) + Translate;
        int countNodes = Nodes.Length;
        Vector2[] globalNodes = new Vector2[countNodes];
        Vector2 maxPoint = new(0, 0), minPoint = new(0, 0);
        for (int i = 0; i < countNodes; i++) {
            if (i == index)
                globalNodes[i] = notRotateNewNode;
            else
                globalNodes[i] = Nodes[i] + Translate;
            maxPoint.X = MathF.Max(maxPoint.X, globalNodes[i].X);
            minPoint.X = MathF.Min(minPoint.X, globalNodes[i].X);
            maxPoint.Y = MathF.Max(maxPoint.Y, globalNodes[i].Y);
            minPoint.Y = MathF.Min(minPoint.Y, globalNodes[i].Y);
        }
        Translate = (minPoint + maxPoint) / 2;
        for (int i = 0; i < countNodes; i++) {
            Nodes[i] = globalNodes[i] - Translate;
        }
        CalcBB();
    }

    public void Resize(int index, Vector2 newNode) {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-Rotate), out Matrix2 result);
        Vector2[] LocalBB = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            LocalBB[i] = result * (BoundingBox[i] - Translate);
        }
        float minSize = 1e-6f;
        Vector2 localNewNode = result * (newNode - Translate);

        int curZero = 0;
        for (int i = 0; i < 4; i++) {
            if (LocalBB[i].X <= 0 && LocalBB[i].Y >= 0) {
                curZero = i;
                break;
            }
        }


        // Костыль!!!
        // Для того, чтобы многоугольник не "схлопывался" в линию
        int znak = 1;
        if (localNewNode.X == 0) {
            if (index == 0 || index == 3)
                znak = -1;
            localNewNode.X += znak * minSize;
        }
        if (localNewNode.Y == 0) {
            if (index == 0 || index == 1)
                znak = 1;
            else if (index == 2 || index == 3)
                znak = -1;
            localNewNode.Y += znak * minSize;
        }

        // Костыль!!!
        // Заново определяем индекс, т.к. в BoundingBox он храниться в другой последовательности
        if (localNewNode.X > 0.0f)
            index = localNewNode.Y > 0.0f ? 1 : 2;
        else
            index = localNewNode.Y > 0.0f ? 0 : 3;
        index = (index + curZero) % 4;
        float wOld = Math.Max(Math.Abs(LocalBB[index].X - LocalBB[(index + 2) % 4].X), minSize);
        float hOld = Math.Max(Math.Abs(LocalBB[index].Y - LocalBB[(index + 2) % 4].Y), minSize);
        float sX = Math.Abs(localNewNode.X - LocalBB[(index + 2) % 4].X) / wOld;
        float sY = Math.Abs(localNewNode.Y - LocalBB[(index + 2) % 4].Y) / hOld;
        for (int i = 0; i < Nodes.Length; i++) {
            Nodes[i].X *= sX;
            Nodes[i].Y *= sY;
        }
        Vector2 deltaDev2 = (newNode - BoundingBox[index]) / 2;
        Translate += deltaDev2;
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
        if (float.IsNaN(angle))
            return;
        int sign = 1;
        if (new Matrix2(p1, p2).Determinant > 0)
            sign = -1;
        Rotate += sign * angle;
        Rotate %= 360;
        CalcBB();
    }

    public void NormalizeIndexNodes() {
        // TODO: Реализовать метод
    }

    public void ReflectX() {
        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = new Vector2(Nodes[i].X, -Nodes[i].Y);
        }

        Rotate = -Rotate;
        Rotate %= 360;
        if (Rotate < 0) Rotate += 360;
        
        CalcBB();
    }

    public void ReflectY() {
        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = new Vector2(-Nodes[i].X, Nodes[i].Y);
        }

        Rotate = 180 - Rotate;
        Rotate %= 360;
        if (Rotate < 0) Rotate += 360;
        
        CalcBB();
    }
}