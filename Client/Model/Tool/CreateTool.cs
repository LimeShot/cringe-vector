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

    public event EventHandler<List<IShape>>? OnShapeChanged = e;
    public string Name { get; set; } = name;

    public void MouseDownEvent(Vector2 startPoint) {
        _startPoint = startPoint;
        Console.WriteLine(startPoint);
        AddShape(_canvas, startPoint);
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (isMousePressed) {
            _canvas.Shapes.Last().Resize(1, currentPoint);
            _isResized = true;
        }
        //OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }
    public void MouseUpEvent(Vector2 endPoint) {
        if (endPoint == _startPoint) {
            _canvas.Shapes.Remove(_canvas.Shapes.Last());
            AddShape(_canvas, endPoint, 100);
            //OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
        } else if (_isResized) {
            _canvas.Shapes.Last().NormalizeIndexNodes();
        }

        if (_shape != null)
            _commandHistory.AddCommand(new CreateCommand(_shape, _canvas));
    }

    private void AddShape(MyCanvas canvas, params object[] parameters) {
        var z = canvas.GetNewZ();
        var fullParams = parameters.Concat(new object[] { z, null }).ToArray();
        var newShape = ShapeFactory.CreateShape(Name, fullParams);
        if (newShape != null) {
            canvas.AddShape(newShape);
            _shape = newShape;
        }
    }

    public void OnChanged() { return; }
}