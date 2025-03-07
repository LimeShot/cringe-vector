namespace CringeCraft.Client.Model.Tool;

using System.Diagnostics;
using OpenTK.Mathematics;

using CringeCraft.Client.Model.Canvas;

//Может стоит сделать отдельный контроллер для определения режима работы этого тула?

public class ChangeTool(string name, MyCanvas canvas) : ITool {
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    private const float NodeSelectionRadius = 4.0f;   // Радиус для активации изменения размера
    private const float RotateActivationRadius = 10.0f; // Радиус для активации поворота

    private readonly MyCanvas _canvas = canvas;
    private Vector2 _startPoint;
    private int _bbIndex = -1;
    private ChangeToolMode _mode = ChangeToolMode.None;

    public string Name { get; } = name;

    public void MouseDownEvent(Vector2 startPoint) {
        _startPoint = startPoint;

        if (_canvas.SelectedShapes.Count == 0) {
            SelectShape(startPoint);
            if (_canvas.SelectedShapes.Count > 0)
                _mode = ChangeToolMode.Move;
        } else {
            _bbIndex = TryGetBBNode(startPoint);
            if (_bbIndex >= 0 && _bbIndex < 4)
                _mode = ChangeToolMode.Resize; // Изменение размера для узлов 0-3
            else if (_bbIndex == 4)
                _mode = ChangeToolMode.Rotate; // Поворот для клика снаружи рядом с углом
            else if (IsPointInsideSelectedShape(startPoint))
                _mode = ChangeToolMode.Move;
            else {
                _canvas.SelectedShapes.Clear();
                _mode = ChangeToolMode.None;
            }
        }

        Debug.WriteLine($"Mode: {_mode}, BB Index: {_bbIndex}, Selected Shapes: {_canvas.SelectedShapes.Count}");
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
                //foreach (var shape in _canvas.SelectedShapes)
                //shape.RotateShape(_startPoint, currentPoint);
                _startPoint = currentPoint;
                break;
        }
    }

    public void MouseUpEvent(Vector2 endPoint) {
        _startPoint = Vector2.Zero;
        _mode = ChangeToolMode.None;
        _bbIndex = -1;
    }

    private void SelectShape(Vector2 point) {
        if (_canvas.Shapes.Count == 0) return;
        foreach (var shape in _canvas.Shapes.Reverse()) {
            if (shape.IsBelongsShape(point, SelectionRadius)) {
                _canvas.SelectedShapes.Add(shape);
                break;
            }
        }
    }

    private bool IsPointInsideSelectedShape(Vector2 point) {
        return _canvas.SelectedShapes.Any(shape => shape.IsBelongsShape(point, SelectionRadius));
    }

    private int TryGetBBNode(Vector2 point) {
        if (_canvas.SelectedShapes.Count != 1) return -1; // Пока поддерживаем только одну фигуру
        var shape = _canvas.SelectedShapes[0];
        var nodes = shape.BoundingBox;
        if (nodes == null || nodes.Length < 4) return -1;

        bool isInside = shape.IsBelongsShape(point, SelectionRadius); // Точная проверка внутри фигуры

        int bbNode = -1;

        for (int i = 0; i < 4; i++) {
            float distance = Vector2.Distance(point, nodes[i]);
            if (distance <= NodeSelectionRadius) {
                bbNode = i;
            } else if (!isInside && distance <= RotateActivationRadius) {
                bbNode = 4;
            }
        }
        return bbNode;
    }

    public void OnChanged() {
        _startPoint = Vector2.Zero;
        _canvas.SelectedShapes.Clear();
        _mode = ChangeToolMode.None;
    }
}

public enum ChangeToolMode {
    None,
    Move,
    Resize,
    Rotate
}