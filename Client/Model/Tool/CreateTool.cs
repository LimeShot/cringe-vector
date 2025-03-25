namespace CringeCraft.Client.Model.Tool;

using OpenTK.Mathematics;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class CreateTool : ObservableObject, ITool {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    private IShape? _shape;
    private Vector2 _startPoint;
    private bool _isResized = false;

    [ObservableProperty]
    private float _defaultRotationAngle = 0;

    [ObservableProperty]
    private int _polygonSides = 6; // Значение по умолчанию — 6 сторон

    private readonly float _delta = 5;
    private bool createdOnLastClick = false;

    public event Action? OnBoundingBoxChanged;
    public event EventHandler<List<IShape>>? OnShapeChanged;
    public string Name { get; set; }

    public CreateTool(string name, MyCanvas canvas, EventHandler<List<IShape>>? e, MyCommandHistory commandHistory) {
        Name = name;
        _canvas = canvas;
        _commandHistory = commandHistory;
        OnShapeChanged = e;
        OnBoundingBoxChanged += canvas.OnCringeBoundingBoxChanged;
    }

    public void MouseDownEvent(Vector2 startPoint, bool isCtrlPressed) {
        _startPoint = startPoint;
        _shape = AddShape(startPoint);
        createdOnLastClick = true;
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (isMousePressed && _shape != null && createdOnLastClick) {
            _shape.Resize(1, currentPoint);
            _isResized = true;
            OnBoundingBoxChanged?.Invoke();
        }
    }
    public void MouseUpEvent(Vector2 endPoint) {
        if ((endPoint - _startPoint).Length < _delta) {
            _canvas.Shapes.Remove(_canvas.Shapes[0]);
            _shape = AddShape(endPoint, 100, DefaultRotationAngle);
        } else if (_isResized && _shape != null) {
            _shape.NormalizeIndexNodes();
            _isResized = false;
        }
        createdOnLastClick = false;
        if (_shape != null)
            _commandHistory.AddCommand(new CreateCommand(_shape, _canvas));
    }

    private IShape? AddShape(params object[] parameters) {
        var z = _canvas.GetNewZ();
        ShapeStyle shapeStyle = new(_canvas.StartOutLineColor, _canvas.StartFillColor, _canvas.HasFill, true);
        var fullParams = parameters.Concat([z, _canvas.DeltaZ, shapeStyle]).ToArray();
        if (Name.Equals("Polygon", StringComparison.OrdinalIgnoreCase)) {
            fullParams = fullParams.Concat([PolygonSides]).ToArray();
        }
        var newShape = ShapeFactory.CreateShape(Name, fullParams);
        if (newShape != null) {
            _canvas.SelectedShapes.Clear();
            OnBoundingBoxChanged?.Invoke();
            _canvas.AddShape(newShape);
            _canvas.SelectedShapes.Add(newShape);
            OnBoundingBoxChanged?.Invoke();
            return newShape;
        }
        return null;
    }

    public void OnChanged() { return; }

    public void MouseWheelEvent(float delta, Vector2 currentPoint) { }
}