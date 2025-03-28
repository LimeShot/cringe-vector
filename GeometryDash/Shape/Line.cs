namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Line")]
[ExportMetadata("Icon", "line.png")]
public partial class Line : IShape {
    public Vector2 Translate { private set; get; }
    public float Z { set; get; }
    public float DeltaZ { set; get; }
    public float Rotate { private set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { private set; get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        Rotate = (float)Math.Round(Rotate, 1);
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        for (int i = 0; i < 2; i++) {
            BoundingBox[i] = result * Nodes[i] + Translate;
        }
    }

    public Line() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        DeltaZ = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[2];
        Nodes = new Vector2[2];
        Nodes[0] = Vector2.Zero;
        Nodes[1] = Vector2.Zero;
        CalcBB();
    }
    public Line(Line other) {
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
        return new Line(this);
    }

    public Line(Vector2 p1, float z, float deltaZ, ShapeStyle? shapeStyle = null) : this() {
        Translate = p1;
        Z = z;
        DeltaZ = deltaZ;
        Style = shapeStyle ?? new();
        CalcBB();
    }

    public Line(Vector2 p1, float length, float rotateAngle, float z, float deltaZ, ShapeStyle? shapeStyle = null) : this() {
        Translate = p1;
        Z = z;
        Rotate = -rotateAngle;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[2];
        Nodes = new Vector2[2];
        float lengthDev2 = length / 2;
        Nodes[0] = new(-lengthDev2, 0.0f);
        Nodes[1] = new(lengthDev2, 0.0f);
        CalcBB();
    }

    public float[] GetLineVertices() {
        if (!Style.Visible) return [];
        float[] args = [Z + DeltaZ, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
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

    public void Move(Vector2 delta) {
        Translate += delta;
        CalcBB();
    }

    public void Resize(int index, Vector2 newNode) {
        double sideToRotate = 1;
        if (index == 0)
            sideToRotate = -1;
        Vector2 deltaDev2 = (newNode - BoundingBox[index]) / 2;
        Vector2 rotateNodes = BoundingBox[index] - Translate + deltaDev2;
        Translate += deltaDev2;
        double denominator = rotateNodes.Length;
        if (Math.Abs(denominator) < 1E-10)
            denominator = 1E-7;
        float angle = (float)MathHelper.RadiansToDegrees(Math.Acos(sideToRotate * rotateNodes.X / denominator));
        if (float.IsNaN(angle))
            return;
        int sign = 1;
        if (sideToRotate * rotateNodes.Y > 0)
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
        if (float.IsNaN(angle))
            return;
        int sign = 1;
        if (new Matrix2(p1, p2).Determinant > 0)
            sign = -1;
        Rotate += sign * angle;
        Rotate %= 180;
        CalcBB();
    }

    public void NormalizeIndexNodes() {
        if (Nodes[0].X > 0.0f) {
            (Nodes[0], Nodes[1]) = (Nodes[1], Nodes[0]);
            CalcBB();
        }
    }

    public void ReflectX() {
        Rotate = -Rotate;
        Rotate = (Rotate % 360 + 360) % 360;
        CalcBB();
    }

    public void ReflectY() {
        Rotate = 180 - Rotate;
        Rotate = (Rotate % 360 + 360) % 360;
        CalcBB();
    }
    public override bool Equals(object? obj) {
        if (obj is not Line other)
            return false;

        return Translate == other.Translate
            && Z == other.Z
            && DeltaZ == other.DeltaZ
            && Rotate == other.Rotate
            && (Style?.Equals(other.Style) ?? other.Style is null)
            && BoundingBox.SequenceEqual(other.BoundingBox)
            && Nodes.SequenceEqual(other.Nodes);
    }

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(Translate);
        hash.Add(Z);
        hash.Add(DeltaZ);
        hash.Add(Rotate);
        hash.Add(Style);
        foreach (var point in BoundingBox) hash.Add(point);
        foreach (var node in Nodes) hash.Add(node);
        return hash.ToHashCode();
    }

    public static bool operator ==(Line? a, Line? b)
        => ReferenceEquals(a, b) || (a is not null && b is not null && a.Equals(b));

    public static bool operator !=(Line? a, Line? b) => !(a == b);
}