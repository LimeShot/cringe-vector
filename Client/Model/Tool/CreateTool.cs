namespace CringeCraft.Client.Model.Tool;

using OpenTK.Mathematics;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class CreateTool(string name, MyCanvas canvas, EventHandler<List<IShape>>? e, MyCommandHistory commandHistory) : ObservableObject, ITool {
    private readonly MyCanvas _canvas = canvas;
    private readonly MyCommandHistory _commandHistory = commandHistory;
    private IShape? _shape;
    private Vector2 _startPoint;
    private bool _isResized = false;

    [ObservableProperty]
    private float _defaultRotationAngle = 0;

    [ObservableProperty]
    private int _polygonSides = 6; // Значение по умолчанию — 6 сторон

    private const float NodeSelectionRadius = 4.0f;
    private bool _isCtrlPressed;
    private readonly float _delta = 5;
    private bool createdOnLastClick = false;

    public event EventHandler<List<IShape>>? OnShapeChanged = e;
    public string Name { get; set; } = name;

    public void MouseDownEvent(Vector2 startPoint, bool isCtrlPressed) {
        _isCtrlPressed = isCtrlPressed;
        _startPoint = startPoint;
        _shape = AddShape(startPoint);
        createdOnLastClick = true;
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (isMousePressed && _shape != null && createdOnLastClick) {
            _shape.Resize(1, currentPoint);
            _isResized = true;
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
        ShapeStyle shapeStyle = new ShapeStyle(_canvas.StartOutLineColor, _canvas.StartFillColor, _canvas.HasFill, true);
        var fullParams = parameters.Concat([z, _canvas.DeltaZ, shapeStyle]).ToArray();
        if (Name.Equals("Polygon", StringComparison.OrdinalIgnoreCase)) {
            fullParams = fullParams.Concat([PolygonSides]).ToArray();
        }
        var newShape = ShapeFactory.CreateShape(Name, fullParams);
        if (newShape != null) {
            _canvas.AddShape(newShape);
            return newShape;
        }
        return null;
    }

    public void OnChanged() { return; }

    public void MouseWheelEvent(float delta, Vector2 currentPoint) { }
}