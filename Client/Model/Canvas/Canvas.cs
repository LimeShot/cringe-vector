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
using System.Diagnostics;

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
    public List<IShape> SelectedShapes = new(); // Список выделенных фигур

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

    [ObservableProperty]
    private Vector2[] _getGeneralBB;
    public Vector2 GetTranslate { get; private set; }
    public float GetRotate { get; private set; }

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


    public void SelectShape(Vector2 point) {
        if (Shapes.Count == 0) return;
        foreach (var shape in Shapes) {
            if (!SelectedShapes.Contains(shape) && shape.Style.Visible && shape.IsBelongsShape(point, SelectionRadius)) {
                SelectedShapes.Add(shape);

                StartOutLineColor = shape.Style.ColorOutline;
                StartFillColor = shape.Style.ColorFill;
                HasFill = shape.Style.Fill;
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
        CalcTranslate(SelectedShapes);
    }

    public bool IsPointInsideSelectedBB(Vector2 point) {
        if (GetGeneralBB is null)
            return false;
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-GetRotate), out Matrix2 result);
        Vector2[] localBB =
        [
            result * (GetGeneralBB[0] - GetTranslate),
            result * (GetGeneralBB[1] - GetTranslate),
            result * (GetGeneralBB[2] - GetTranslate),
            result * (GetGeneralBB[3] - GetTranslate),
        ];
        var min = localBB.First();
        var max = localBB.First();
        for (int i = 1; i < localBB.Length; i++) {
            var node = localBB[i];
            min = Vector2.ComponentMin(node, min);
            max = Vector2.ComponentMax(node, max);
        }
        var localPoint = result * (point - GetTranslate);
        return localPoint.X >= min.X && localPoint.X <= max.X &&
            localPoint.Y >= min.Y && localPoint.Y <= max.Y;
    }

    public void CalcGeneralBoundingBox(List<IShape> shapeBuffer) {
        Vector2 point3 = (
            shapeBuffer.Select(shape => shape.BoundingBox.Min(v => v.X)).Min(),
            shapeBuffer.Select(shape => shape.BoundingBox.Min(v => v.Y)).Min()
        );
        Vector2 point1 = (
            shapeBuffer.Select(shape => shape.BoundingBox.Max(v => v.X)).Max(),
            shapeBuffer.Select(shape => shape.BoundingBox.Max(v => v.Y)).Max()
        );
        Vector2 point0 = (point3.X, point1.Y);
        Vector2 point2 = (point1.X, point3.Y);
        GetGeneralBB = [point0, point1, point2, point3];
    }

    public void CalcTranslate(List<IShape> shapeBuffer) {
        if (shapeBuffer.Count == 1) {
            var shape = shapeBuffer.First();
            GetGeneralBB = shape.BoundingBox;
            GetTranslate = shape.Translate;
            GetRotate = shape.Rotate;
            OnPropertyChanged(nameof(GetGeneralBB));
        } else if (shapeBuffer.Count > 1) {
            CalcGeneralBoundingBox(shapeBuffer);
            GetRotate = 0.0f;
            GetTranslate = (GetGeneralBB[0] + GetGeneralBB[2]) / 2f;
            OnPropertyChanged(nameof(GetGeneralBB));
        } else {
            GetRotate = 0.0f;
            GetTranslate = new Vector2(0f, 0f);
            GetGeneralBB = null;
        }
    }

    public void ChangeOutLineColor(Color color) {
        var new_color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

        if (SelectedShapes.Count > 0) {
            foreach (IShape shape in SelectedShapes) {
                var shape_color = Color.FromRgb(
                    (byte)(shape.Style.ColorOutline.X * 255f),
                    (byte)(shape.Style.ColorOutline.Y * 255f),
                    (byte)(shape.Style.ColorOutline.Z * 255f)
                );
                if (shape_color != SelectedOutlineColor) {
                    shape.Style.ColorOutline = new_color;
                } else {
                    flag_outlineColor = false;
                }
            }
        }
        OnShapeChanged?.Invoke(this, SelectedShapes);
        if (!flag_outlineColor) {
            flag_outlineColor = true;
            return;
        }
        StartOutLineColor = new_color;
    }

    public void ChangeFillColor(Color color) {
        var new_color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

        if (SelectedShapes.Count > 0) {
            foreach (IShape shape in SelectedShapes) {
                var shape_color = Color.FromRgb(
                    (byte)(shape.Style.ColorFill.X * 255f),
                    (byte)(shape.Style.ColorFill.Y * 255f),
                    (byte)(shape.Style.ColorFill.Z * 255f)
                );
                if (shape_color != SelectedFillColor) {
                    shape.Style.ColorFill = new_color;
                } else {
                    flag_fillColor = false;
                }
            }
        }
        OnShapeChanged?.Invoke(this, SelectedShapes);
        if (!flag_fillColor) {
            flag_fillColor = true;
            return;
        }
        StartFillColor = new_color;
    }

    private void ChangeFill(object? sender, PropertyChangedEventArgs e) {
        if (SelectedShapes.Count > 1) { return; }
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

    public void OnCringeBoundingBoxChanged() => CalcTranslate(SelectedShapes);

    public void CringeEvent() => OnShapeChanged?.Invoke(this, SelectedShapes);
}