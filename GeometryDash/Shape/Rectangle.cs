namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Rectangle")]
[ExportMetadata("Icon", "rectangle.png")]
public partial class Rectangle : IShape {
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
        for (int i = 0; i < 4; i++) {
            BoundingBox[i] = result * Nodes[i] + Translate;
        }
    }

    public Rectangle() {
        Translate = Vector2.Zero;
        Z = 0.0f;
        DeltaZ = 0.0f;
        Rotate = 0.0f;
        Style = new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            Nodes[i] = Vector2.Zero;
        }
        CalcBB();
    }

    public Rectangle(Rectangle other) {
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
        return new Rectangle(this);
    }

    public Rectangle(Vector2 p1, float z, float deltaZ, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
        DeltaZ = deltaZ;
        Rotate = 0.0f;
        Style = shapeStyle ?? new();
        BoundingBox = new Vector2[4];
        Nodes = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            Nodes[i] = Vector2.Zero;
        }
        CalcBB();
    }

    public Rectangle(Vector2 p1, float side, float rotateAngle, float z, float deltaZ, ShapeStyle shapeStyle) {
        Translate = p1;
        Z = z;
        DeltaZ = deltaZ;
        Rotate = rotateAngle;
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
        if (!Style.Visible) return [];
        float[] args = [Z + DeltaZ, Translate.X, Translate.Y, Rotate, Style.ColorOutline.X, Style.ColorOutline.Y, Style.ColorOutline.Z];
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
        if (!Style.Visible || !Style.Fill) return [];
        float[] args = [Z, Translate.X, Translate.Y, Rotate, Style.ColorFill.X, Style.ColorFill.Y, Style.ColorFill.Z];
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
        float halfWidth = (Nodes[1].X - Nodes[0].X) / 2.0f + radiusPoint;
        float halfHeight = (Nodes[1].Y - Nodes[3].Y) / 2.0f + radiusPoint;

        return xRot >= -halfWidth && xRot <= halfWidth &&
               yRot >= -halfHeight && yRot <= halfHeight;
    }

    public void Move(Vector2 delta) {
        Translate += delta;
        CalcBB();
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
        bool change = false;
        if (Nodes[0].X - Nodes[1].X > 0.0f && MathF.Abs(Nodes[0].X - Nodes[1].X) > 1E-6) {
            (Nodes[0], Nodes[1]) = (Nodes[1], Nodes[0]);
            (Nodes[3], Nodes[2]) = (Nodes[2], Nodes[3]);
            change = true;
        }
        if (Nodes[0].Y - Nodes[3].Y < 0.0f && MathF.Abs(Nodes[0].Y - Nodes[3].Y) > 1E-6) {
            (Nodes[1], Nodes[2]) = (Nodes[2], Nodes[1]);
            (Nodes[0], Nodes[3]) = (Nodes[3], Nodes[0]);
            change = true;
        }
        if (change)
            CalcBB();
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
        if (obj is not Rectangle other)
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

    public static bool operator ==(Rectangle? a, Rectangle? b)
        => ReferenceEquals(a, b) || (a is not null && b is not null && a.Equals(b));

    public static bool operator !=(Rectangle? a, Rectangle? b) => !(a == b);
}