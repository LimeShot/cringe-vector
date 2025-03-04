using System.Numerics;
using System.Windows;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Tool;
public class CreateTool(string name, MyCanvas canvas, EventHandler<List<IShape>>? e) : ITool {
    private readonly MyCanvas _canvas = canvas;
    private Point _startPoint;
    public event EventHandler<List<IShape>>? OnShapeChanged = e;

    public string Name { get; set; } = name;

    public void MouseDownEvent(Point startPoint) {
        _startPoint = startPoint;
        AddShape(_canvas, new OpenTK.Mathematics.Vector2((float)startPoint.X, (float)startPoint.Y));
    }

    public void MouseMoveEvent(Point currentPoint) {
        _canvas.Shapes[_canvas.Shapes.Count - 1].Resize(1, new OpenTK.Mathematics.Vector2((float)currentPoint.X, (float)currentPoint.Y));
        //OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }
    public void MouseUpEvent(Point endPoint) {
        if (endPoint == _startPoint) {
            _canvas.Shapes.Remove(_canvas.Shapes[_canvas.Shapes.Count - 1]);
            AddShape(_canvas, new OpenTK.Mathematics.Vector2((float)_startPoint.X, (float)_startPoint.Y), 25);
            //OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
        }
    }

    private void AddShape(MyCanvas canvas, params object[] parameters) {
        var z = canvas.GetNewZ();
        var fullParams = parameters.Concat(new object[] { z, null }).ToArray();
        var newShape = ShapeFactory.CreateShape(Name, fullParams);
        if (newShape != null)
            canvas.AddShape(newShape);
    }

    public void OnChanged() { return; }
}