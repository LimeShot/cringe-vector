namespace CringeCraft.Client.Model.Tool;

using System.Windows.Input;
using OpenTK.Mathematics;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Commands.CommandHistory;

public class CreateTool(string name, MyCanvas canvas, EventHandler<List<IShape>>? e, MyCommandHistory commandHistory) : ITool {
    private readonly MyCanvas _canvas = canvas;
    private readonly MyCommandHistory _commandHistory = commandHistory;
    private IShape? _shape;
    private Vector2 _startPoint;
    private bool _isResized = false;
    private int bbIndex = 1;

    //Кринж?
    private const float NodeSelectionRadius = 4.0f;   // Радиус для активации изменения размера

    public event EventHandler<List<IShape>>? OnShapeChanged = e;
    public string Name { get; set; } = name;

    public void MouseDownEvent(Vector2 startPoint) {
        _startPoint = startPoint;
        Console.WriteLine(startPoint);
        _shape = AddShape(startPoint);
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (isMousePressed && _shape != null) {
            GetBBNode(currentPoint, _shape);
            _shape.Resize(bbIndex, currentPoint);
            _isResized = true;
        }
    }

    public void MouseUpEvent(Vector2 endPoint) {
        if (endPoint == _startPoint) {
            _canvas.Shapes.Remove(_canvas.Shapes.Last());
            _shape = AddShape(endPoint, 100);
        } else if (_isResized && _shape != null) {
            _shape.NormalizeIndexNodes();
            _isResized = false;
        }
        bbIndex = 1;
        _commandHistory.AddCommand(new CreateCommand(_shape, _canvas));
    }

    private IShape? AddShape(params object[] parameters) {
        var z = _canvas.GetNewZ();
        var fullParams = parameters.Concat(new object[] { z, null }).ToArray();
        var newShape = ShapeFactory.CreateShape(Name, fullParams);
        if (newShape != null) {
            _canvas.AddShape(newShape);
            return newShape;
        }
        return null;
    }

    private int GetBBNode(Vector2 point, IShape shape) {
        var nodes = shape.BoundingBox;
        for (int i = 0; i < nodes.Count(); i++) {
            float distance = Vector2.Distance(point, nodes[i]);
            if (distance <= NodeSelectionRadius) {
                bbIndex = i;
            }
        }
        return bbIndex;
    }

    public void OnChanged() { return; }

    public void MouseWheelEvent(float delta, Vector2 currentPoint) {
    }
}