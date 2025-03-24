namespace CringeCraft.Client.Model.Canvas;

using CringeCraft.GeometryDash.Shape;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CringeCraft.GeometryDash;
using OpenTK.Mathematics;
using System.IO.Compression;
using System.ComponentModel;

/// <summary>
/// Представляет холст для хранения фигур
/// </summary>
public partial class MyCanvas : ObservableObject, ICanvas {
    [ObservableProperty]
    private ObservableCollection<IShape> _shapes;
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    private const float _stepZ = 0.00002f;
    public readonly float DeltaZ = 0.00001f;

    public Vector3 StartOutLineColor = Vector3.Zero;
    public Vector3 StartFillColor = Vector3.Zero;

    [ObservableProperty]
    public bool _hasFill = false;
    public readonly List<IShape> SelectedShapes = new(); // Список выделенных фигур

    [ObservableProperty]
    private float _width;
    [ObservableProperty]
    private float _height;
    public float ScreenPerWorld = 1.0f;
    public event EventHandler<List<IShape>>? OnShapeChanged;

    public MyCanvas() {
        Shapes = new();
        Width = 2000;
        Height = 1000;
        PropertyChanged += ChangeFill;
    }

    private void recalculateZFromRemove(int index) {
        for (int i = index; i < Shapes.Count; i++) {
            Shapes[i].Z -= _stepZ;
        }
    }

    public float GetNewZ() {
        return Shapes.Count * _stepZ;
    }

    public void AddShape(IShape shape) {
        Shapes.Add(shape);
    }

    public void RemoveShape(IShape shape) {
        int index = Shapes.IndexOf(shape);
        Shapes.Remove(shape);
        recalculateZFromRemove(index);
    }

    public void SaveCanvas() {
        //
    }

    public void SelectShape(Vector2 point) {
        if (Shapes.Count == 0) return;
        foreach (var shape in Shapes.Reverse()) {
            if (shape.Style.Visible && shape.IsBelongsShape(point, SelectionRadius)) {
                SelectedShapes.Add(shape);
                break;
            }
        }
    }

    public bool IsPointInsideSelectedBB(Vector2 point) {
        foreach (var shape in SelectedShapes) {
            var min = shape.Nodes.First();
            var max = shape.Nodes.First();
            for (int i = 1; i < shape.Nodes.Length; i++) {
                var node = shape.Nodes[i];
                min = Vector2.ComponentMin(node, min);
                max = Vector2.ComponentMax(node, max);
            }
            Matrix2.CreateRotation(MathHelper.DegreesToRadians(-shape.Rotate), out Matrix2 result);
            var localPoint = result * (point - shape.Translate);
            if (localPoint.X >= min.X && localPoint.X <= max.X &&
                localPoint.Y >= min.Y && localPoint.Y <= max.Y) {
                return true;
            }
        }
        return false;
    }

    public Vector2[] GetGeneralBoundingBox(List<IShape> shapeBuffer) {
        if (shapeBuffer.Count == 0) {
            return null;
        }
        Vector2 point1 = new Vector2(
            shapeBuffer.Select(shape => shape.BoundingBox.Min(v => v.X)).Min(),
            shapeBuffer.Select(shape => shape.BoundingBox.Min(v => v.Y)).Min()
        );
        Vector2 point3 = new Vector2(
            shapeBuffer.Select(shape => shape.BoundingBox.Max(v => v.X)).Max(),
            shapeBuffer.Select(shape => shape.BoundingBox.Max(v => v.Y)).Max()
        );
        Vector2 point2 = new(point1.X, point3.Y);
        Vector2 point4 = new(point3.X, point1.Y);
        return [point1, point2, point3, point4];
    }

    public Vector2 GetCenterOfGBB(List<IShape> shapeBuffer) {
        Vector2[] boundingBox = GetGeneralBoundingBox(shapeBuffer);
        float deltaX = Math.Abs(boundingBox[3].X - boundingBox[0].X);
        float deltaY = Math.Abs(boundingBox[3].Y - boundingBox[0].Y);
        return new Vector2(boundingBox[0].X + deltaX / 2, boundingBox[0].Y - deltaY / 2);
    }

    public void ChangeOutLineColor(Vector3 color) {
        if (SelectedShapes.Count > 0) {
            foreach (IShape shape in SelectedShapes)
                shape.Style.ColorOutline = color;
        }
        StartOutLineColor = color;
    }

    public void ChangeFillColor(Vector3 color) {
        if (SelectedShapes.Count > 0) {
            foreach (IShape shape in SelectedShapes) {
                shape.Style.ColorFill = color;
                shape.Style.Fill = HasFill;
            }
        }
        StartFillColor = color;
    }

    private void ChangeFill(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(HasFill))
            foreach (IShape shape in SelectedShapes)
                shape.Style.Fill = HasFill;

        OnShapeChanged?.Invoke(this, SelectedShapes);
    }
}