namespace CringeCraft.Client.Model.Canvas;

using CringeCraft.GeometryDash.Shape;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CringeCraft.GeometryDash;
using OpenTK.Mathematics;
using System.IO.Compression;

/// <summary>
/// Представляет холст для хранения фигур
/// </summary>
public partial class MyCanvas : ObservableObject, ICanvas {
    public ObservableCollection<IShape> Shapes { get; set; }
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры

    private const float _stepZ = 0.00002f;

    public readonly List<IShape> SelectedShapes = new(); // Список выделенных фигур

    [ObservableProperty]
    private float _width;
    [ObservableProperty]
    private float _height;
    public float ScreenPerWorld = 1.0f;

    public MyCanvas() {
        Shapes = new();
        Width = 2000;
        Height = 1000;
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
            if (shape.IsBelongsShape(point, SelectionRadius)) {
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
}