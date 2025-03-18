namespace CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

using System.Composition;

[Export(typeof(IShape))]
[ExportMetadata("Name", "Polygon")]
[ExportMetadata("Icon", "polygon.png")]
public partial class Polygon : IShape {
    // TODO: Добавить event OnChange в методы set
    public Vector2 Translate { private set; get; }
    public float Z { set; get; }
    public float Rotate { private set; get; }
    public ShapeStyle Style { set; get; }
    public Vector2[] BoundingBox { private set; get; }
    public Vector2[] Nodes { set; get; }

    private void CalcBB() {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        Vector2 maxPoint = new(0, 0), minPoint = new(0, 0);
        foreach (var node in Nodes) {
            maxPoint.X = MathF.Max(maxPoint.X, node.X);
            minPoint.X = MathF.Min(minPoint.X, node.X);
            maxPoint.Y = MathF.Max(maxPoint.Y, node.Y);
            minPoint.Y = MathF.Min(minPoint.Y, node.Y);
        }
        BoundingBox[0] = new(minPoint.X, maxPoint.Y);
        BoundingBox[1] = maxPoint;
        BoundingBox[2] = new(maxPoint.X, minPoint.Y);
        BoundingBox[3] = minPoint;
        for (int i = 0; i < 4; i++) {
            BoundingBox[i] = result * BoundingBox[i] + Translate;
        }
    }

    public Polygon() {
        Translate = Vector2.Zero;
        Z = 0.0f;
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

    public Polygon(Vector2 p1, float z, ShapeStyle? shapeStyle = null) {
        float diagonal = 1e-5f;
        Translate = p1;
        Z = z;
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

    public Polygon(Vector2 p1, float diagonal, float z, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
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

    public Polygon(Vector2 p1, float diagonal, int countVertices, float z, ShapeStyle? shapeStyle = null) {
        Translate = p1;
        Z = z;
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
            returns[i * 18 + 2] = Z;
            returns[i * 18 + 3] = Translate.X;
            returns[i * 18 + 4] = Translate.Y;
            returns[i * 18 + 5] = Rotate;
            returns[i * 18 + 6] = Style.ColorOutline.X;
            returns[i * 18 + 7] = Style.ColorOutline.Y;
            returns[i * 18 + 8] = Style.ColorOutline.Z;

            returns[i * 18 + 9] = Nodes[(i + 1) % len].X;
            returns[i * 18 + 10] = Nodes[(i + 1) % len].Y;
            returns[i * 18 + 11] = Z;
            returns[i * 18 + 12] = Translate.X;
            returns[i * 18 + 13] = Translate.Y;
            returns[i * 18 + 14] = Rotate;
            returns[i * 18 + 15] = Style.ColorOutline.X;
            returns[i * 18 + 16] = Style.ColorOutline.Y;
            returns[i * 18 + 17] = Style.ColorOutline.Z;
        }
        return returns;


        //
        // Пока нету шейдера на отрисовку поворота, можно юзать, но надо закоментить то, что сверху 


        // Matrix2.CreateRotation(MathHelper.DegreesToRadians(Rotate), out Matrix2 result);
        // Vector2[] rotateNode = new Vector2[Nodes.Length];
        // for (int i = 0; i < Nodes.Length; i++) {
        //     rotateNode[i] = result * Nodes[i];
        // }

        // int len = Nodes.Length;
        // float[] returns = new float[len * 2 * 9];
        // for (int i = 0; i < len; i++) {
        //     returns[i * 18] = rotateNode[i].X;
        //     returns[i * 18 + 1] = rotateNode[i].Y;
        //     returns[i * 18 + 2] = Z;
        //     returns[i * 18 + 3] = Translate.X;
        //     returns[i * 18 + 4] = Translate.Y;
        //     returns[i * 18 + 5] = Rotate;
        //     returns[i * 18 + 6] = Style.ColorOutline.X;
        //     returns[i * 18 + 7] = Style.ColorOutline.Y;
        //     returns[i * 18 + 8] = Style.ColorOutline.Z;

        //     returns[i * 18 + 9] = rotateNode[(i + 1) % len].X;
        //     returns[i * 18 + 10] = rotateNode[(i + 1) % len].Y;
        //     returns[i * 18 + 11] = Z;
        //     returns[i * 18 + 12] = Translate.X;
        //     returns[i * 18 + 13] = Translate.Y;
        //     returns[i * 18 + 14] = Rotate;
        //     returns[i * 18 + 15] = Style.ColorOutline.X;
        //     returns[i * 18 + 16] = Style.ColorOutline.Y;
        //     returns[i * 18 + 17] = Style.ColorOutline.Z;
        // }
        // return returns;

    }

    public float[] GetTriangleVertices() {
        // TODO: Сделать триангуляцию
        return [];
    }

    public float[] GetCircumferenceVertices() {
        return [];
    }

    public float[] GetCircleVertices() {
        return [];
    }

    public bool IsBelongsShape(Vector2 point, float radiusPoint) {
        // TODO: Переделать (Метод временно сделан через LocalBoundingBox)
        Vector2 localPoint = point - Translate;

        float angle = -MathHelper.DegreesToRadians(Rotate);
        float xRot = localPoint.X * MathF.Cos(angle) - localPoint.Y * MathF.Sin(angle);
        float yRot = localPoint.X * MathF.Sin(angle) + localPoint.Y * MathF.Cos(angle);
        localPoint = new Vector2(xRot, yRot);
        Vector2 maxPoint = new(0, 0), minPoint = new(0, 0);
        foreach (var node in Nodes) {
            if (node.X > maxPoint.X)
                maxPoint.X = node.X;
            else if (node.X < minPoint.X)
                minPoint.X = node.X;
            if (node.Y > maxPoint.Y)
                maxPoint.Y = node.Y;
            else if (node.Y < minPoint.Y)
                minPoint.Y = node.Y;
        }
        float halfWidth = (maxPoint.X - minPoint.X) / 2.0f + radiusPoint;
        float halfHeight = (maxPoint.Y - minPoint.Y) / 2.0f + radiusPoint;

        return xRot >= -halfWidth && xRot <= halfWidth &&
               yRot >= -halfHeight && yRot <= halfHeight;
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
        // TODO: Реализовать метод
    }

    public void Resize(int index, Vector2 newNode) {
        // TODO: Исправить на нормальный ресайз
        if (newNode.X > Translate.X)
            index = newNode.Y > Translate.Y ? 1 : 2;
        else
            index = newNode.Y > Translate.Y ? 0 : 3;

        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-Rotate), out Matrix2 result);
        Vector2[] LocalBB = new Vector2[4];
        for (int i = 0; i < 4; i++) {
            LocalBB[i] = result * (BoundingBox[i] - Translate);
        }
        Vector2 localNewNode = result * (newNode - Translate);
        float wOld = Math.Max(Math.Abs(LocalBB[index].X - LocalBB[(index + 2) % 4].X), 1e-6f);
        float hOld = Math.Max(Math.Abs(LocalBB[index].Y - LocalBB[(index + 2) % 4].Y), 1e-6f);
        float sX = Math.Abs(localNewNode.X - LocalBB[(index + 2) % 4].X) / wOld;
        float sY = Math.Abs(localNewNode.Y - LocalBB[(index + 2) % 4].Y) / hOld;
        for (int i = 0; i < Nodes.Length; i++) {
            Nodes[i].X *= sX;
            Nodes[i].Y *= sY;
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
        Rotate %= 360;
        CalcBB();
    }

    public void NormalizeIndexNodes() {
        // TODO: Реализовать метод
    }

    public void Reflect() {
        // TODO: Реализовать метод
    }
}