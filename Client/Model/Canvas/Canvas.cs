namespace CringeCraft.Client.Model.Canvas;

using CringeCraft.GeometryDash.Shape;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CringeCraft.GeometryDash;
using OpenTK.Mathematics;
using System.IO.Compression;
using System.ComponentModel;
using System.Windows.Media;
using CringeCraft.Client.Model.Commands.CommandHistory;
using System.Windows;

/// <summary>
/// Представляет холст для хранения фигур
/// </summary>
public partial class MyCanvas : ObservableObject, ICanvas {
    [ObservableProperty]
    private ObservableCollection<IShape> _shapes;
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    private const float _stepZ = 0.00002f;
    public readonly float DeltaZ = 0.00001f;
    private bool flag_outlineColor = true;
    private bool flag_fillColor = true;

    [ObservableProperty]
    private Color _selectedOutlineColor = Colors.Black;

    [ObservableProperty]
    private Color _selectedFillColor = Colors.Black;

    public Vector3 StartOutLineColor = Vector3.Zero;
    public Vector3 StartFillColor = Vector3.Zero;

    [ObservableProperty]
    public bool _hasFill = false;
    public readonly List<IShape> SelectedShapes = new(); // Список выделенных фигур

    [ObservableProperty]
    private float _width;

    [ObservableProperty]
    private float _height;

    [ObservableProperty]
    private string _canvasSize;

    [ObservableProperty]
    private Visibility _isShapesVisible;

    public float ScreenPerWorld = 1.0f;
    public event EventHandler<List<IShape>>? OnShapeChanged;
    public Vector2[] GetGeneralBB { get; private set; }
    public Vector2 GetTranslate { get; private set; }

    private readonly MyCommandHistory _myCommandHistory;

    public MyCanvas(MyCommandHistory myCommandHistory) {
        Shapes = new();
        Width = 2000;
        Height = 1000;
        CanvasSize = $"Холст: {Width}x{Height}";
        IsShapesVisible = Visibility.Hidden;
        GetGeneralBB = new Vector2[4];
        _myCommandHistory = myCommandHistory;

        PropertyChanged += ChangeFill;
        Shapes.CollectionChanged += (s, e) =>
            IsShapesVisible = Shapes.Count > 0 ? Visibility.Visible : Visibility.Hidden;
    }

    private void recalculateZFromRemove(int index) {
        for (int i = index; i < Shapes.Count; i++) {
            Shapes[i].Z -= _stepZ;
        }
    }

    public float GetNewZ() {
        return Shapes.Count * _stepZ;
    }

    public void RecalculateAllZ() {
        float z = 0;
        foreach (IShape shape in Shapes.Reverse()) {
            shape.Z = z;
            z += _stepZ;
        }
        OnShapeChanged?.Invoke(this, [.. Shapes]);
    }

    public void AddShape(IShape shape) {
        Shapes.Insert(0, shape);
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
        foreach (var shape in Shapes) {
            if (shape.Style.Visible && shape.IsBelongsShape(point, SelectionRadius)) {
                SelectedShapes.Add(shape);

                StartOutLineColor = shape.Style.ColorOutline;
                StartFillColor = shape.Style.ColorFill;
                HasFill = shape.Style.Fill;
                flag_outlineColor = false;
                flag_fillColor = false;
                SelectedOutlineColor = Color.FromRgb(
                    (byte)(StartOutLineColor.X * 255f),
                    (byte)(StartOutLineColor.Y * 255f),
                    (byte)(StartOutLineColor.Z * 255f)
                );
                SelectedFillColor = Color.FromRgb(
                    (byte)(StartFillColor.X * 255f),
                    (byte)(StartFillColor.Y * 255f),
                    (byte)(StartFillColor.Z * 255f)
                );

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

    public Vector2[] CalcGeneralBoundingBox(List<IShape> shapeBuffer) {
        if (shapeBuffer.Count == 0) {
            return null;
        }
        Vector2 point3 = new Vector2(
            shapeBuffer.Select(shape => shape.BoundingBox.Min(v => v.X)).Min(),
            shapeBuffer.Select(shape => shape.BoundingBox.Min(v => v.Y)).Min()
        );
        Vector2 point1 = new Vector2(
            shapeBuffer.Select(shape => shape.BoundingBox.Max(v => v.X)).Max(),
            shapeBuffer.Select(shape => shape.BoundingBox.Max(v => v.Y)).Max()
        );
        Vector2 point0 = new(point3.X, point1.Y);
        Vector2 point2 = new(point1.X, point3.Y);
        GetGeneralBB = [point0, point1, point2, point3];
        return [point0, point1, point2, point3];
    }

    public Vector2 CalcTranslate(List<IShape> shapeBuffer) {
        Vector2[] boundingBox = CalcGeneralBoundingBox(shapeBuffer);
        float deltaX = boundingBox[1].X - boundingBox[3].X;
        float deltaY = boundingBox[1].Y - boundingBox[3].Y;
        GetTranslate = (boundingBox[0].X + deltaX / 2, boundingBox[0].Y - deltaY / 2);
        return new Vector2(boundingBox[0].X + deltaX / 2, boundingBox[0].Y - deltaY / 2);
    }

    public void ChangeOutLineColor(Color color) {
        if (!flag_outlineColor) {
            flag_outlineColor = true;
            return;
        }

        var new_color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

        if (SelectedShapes.Count > 0) {
            foreach (IShape shape in SelectedShapes) {
                shape.Style.ColorOutline = new_color;
            }
        }
        StartOutLineColor = new_color;
    }

    public void ChangeFillColor(Color color) {
        if (!flag_fillColor) {
            flag_fillColor = true;
            return;
        }

        var new_color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

        if (SelectedShapes.Count > 0) {
            foreach (IShape shape in SelectedShapes) {
                shape.Style.ColorFill = new_color;
                shape.Style.Fill = HasFill;
            }
        }
        StartFillColor = new_color;

    }

    private void ChangeFill(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(HasFill))
            foreach (IShape shape in SelectedShapes)
                shape.Style.Fill = HasFill;

        OnShapeChanged?.Invoke(this, SelectedShapes);
    }

    public void MoveShapeUp(IShape shape) {
        var index = Shapes.IndexOf(shape);

        (Shapes[index - 1].Z, shape.Z) = (shape.Z, Shapes[index - 1].Z);
        (Shapes[index - 1], Shapes[index]) = (Shapes[index], Shapes[index - 1]);
    }

    public void MoveShapeDown(IShape shape) {
        var index = Shapes.IndexOf(shape);

        (Shapes[index + 1].Z, shape.Z) = (shape.Z, Shapes[index + 1].Z);
        (Shapes[index + 1], Shapes[index]) = (Shapes[index], Shapes[index + 1]);
    }

    public void DeleteAllShapes() {
        SelectedShapes.Clear();
        _myCommandHistory.AddCommand(new DeleteAllCommand(Shapes));
        Shapes.Clear();
    }
}