namespace CringeCraft.Client.Model.Tool;

using System.Diagnostics;
using OpenTK.Mathematics;

using CringeCraft.Client.Model.Canvas;

public class ChangeTool(string name, MyCanvas canvas) : ITool {
    private readonly MyCanvas _canvas = canvas;
    private Vector2 _startPoint;
    private int _bbIndex = -1;
    private ChangeToolMode _mode = ChangeToolMode.None;

    public string Name { get; set; } = name;

    public enum ChangeToolMode {
        None,
        Move,
        Resize,
        Rotate
    }

    public void OnChanged() {
        _startPoint = Vector2.Zero;
        _canvas.SelectedShapes.Clear();
    }

    public void MouseDownEvent(Vector2 startPoint) {
        _startPoint = startPoint;

        if (_canvas.SelectedShapes.Count == 0) {
            SelectShape(startPoint, 1.0f);
            if (_canvas.SelectedShapes.Count > 0)
                _mode = ChangeToolMode.Move; // По умолчанию — перемещение для новой фигуры
        } else {
            _bbIndex = TryGetBBNode(startPoint, 5.0f);
            if (_bbIndex >= 0 && _bbIndex < 4)
                _mode = ChangeToolMode.Resize;
            else if (_bbIndex == 4)
                _mode = ChangeToolMode.Rotate;
            else if (IsPointInsideSelectedShape(startPoint))
                _mode = ChangeToolMode.Move;
            else {
                _canvas.SelectedShapes.Clear();
                _mode = ChangeToolMode.None;
            }
        }

        Debug.WriteLine($"Mode: {_mode}, Selected Shapes: {_canvas.SelectedShapes.Count}");
    }

    public void MouseMoveEvent(Vector2 currentPoint) {
        if (_canvas.SelectedShapes.Count == 0) return;

        switch (_mode) {
            case ChangeToolMode.Move:
                var delta = currentPoint - _startPoint;
                foreach (var shape in _canvas.SelectedShapes)
                    shape.Move(delta);
                _startPoint = currentPoint;
                break;

            case ChangeToolMode.Resize:
                foreach (var shape in _canvas.SelectedShapes)
                    shape.Resize(_bbIndex, currentPoint);
                break;

            case ChangeToolMode.Rotate:
                foreach (var shape in _canvas.SelectedShapes)
                    //shape.RotateShape(_bbIndex, currentPoint);
                    _startPoint = currentPoint;
                break;
        }
    }
    public void MouseUpEvent(Vector2 endPoint) {
        _mode = ChangeToolMode.None; // Сброс режима после завершения действия
        _bbIndex = -1;
    }

    private void SelectShape(Vector2 point, float radius) {
        foreach (var shape in _canvas.Shapes.Reverse()) {
            if (shape.IsBelongsShape(point, radius)) {
                _canvas.SelectedShapes.Add(shape);
                break;
            }
        }
    }

    private bool IsPointInsideSelectedShape(Vector2 point) {
        return _canvas.SelectedShapes.Any(shape => shape.IsBelongsShape(point, 1.0f));
    }

    private int TryGetBBNode(Vector2 point, float radiusPoint) { //Работает при условии, что в Selected Shape один элемент
        int bbNode = -1;
        float minDistance = 52;

        foreach (var shape in _canvas.SelectedShapes) {
            for (int i = 0; i < shape.BoundingBox.Length; i++) {
                float distance = Vector2.Distance(point, shape.BoundingBox[i]);
                if (distance <= radiusPoint && distance < minDistance) {
                    minDistance = distance;
                    bbNode = i;
                }
            }
        }
        return bbNode;
    }
}