namespace CringeCraft.Client.Model.Tool;

using System.Diagnostics;
using OpenTK.Mathematics;

using CringeCraft.Client.Model.Canvas;
using System.Windows.Shapes;
using CringeCraft.Client.Model.Commands.CommandHistory;

//Может стоит сделать отдельный контроллер для определения режима работы этого тула?

public class ChangeTool(string name, MyCanvas canvas, MyCommandHistory commandHistory) : ITool {
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    private const float NodeSelectionRadius = 4.0f;   // Радиус для активации изменения размера
    private const float RotateActivationRadius = 10.0f; // Радиус для активации поворота

    private readonly MyCanvas _canvas = canvas;
    private readonly MyCommandHistory _commandHistory = commandHistory;
    private Vector2 _startPoint;
    private Vector2 _firstPoint;
    public int bbIndex { get; private set; } = -1;
    public ChangeToolMode Mode { get; private set; } = ChangeToolMode.None;
    private bool _isResized = false;

    public string Name { get; } = name;

    public void MouseDownEvent(Vector2 startPoint) {
        _startPoint = startPoint;
        _firstPoint = startPoint;

        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(startPoint);
            if (_canvas.SelectedShapes.Count > 0)
                Mode = ChangeToolMode.Move;
        } else {
            SelectMode(startPoint);
            if (Mode == ChangeToolMode.None) {
                _canvas.SelectedShapes.Clear();
                _canvas.SelectShape(startPoint);
                if (_canvas.SelectedShapes.Count > 0)
                    Mode = ChangeToolMode.Move;
            }
        }

        Debug.WriteLine($"Mode: {Mode}, BB Index: {bbIndex}, Selected Shapes: {_canvas.SelectedShapes.Count}");
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (_canvas.SelectedShapes.Count == 0) return;


        if (isMousePressed) {
            switch (Mode) {
                case ChangeToolMode.Move:
                    var delta = currentPoint - _startPoint;
                    foreach (var shape in _canvas.SelectedShapes)
                        shape.Move(delta);
                    _startPoint = currentPoint;
                    break;

                case ChangeToolMode.Resize:
                    foreach (var shape in _canvas.SelectedShapes)
                        shape.Resize(bbIndex, currentPoint);
                    _isResized = true;
                    break;

                case ChangeToolMode.Rotate:
                    foreach (var shape in _canvas.SelectedShapes)
                        shape.RotateShape(_startPoint, currentPoint);
                    _startPoint = currentPoint;
                    break;
            }
        } else {
            SelectMode(currentPoint);
        }
    }

    public void MouseUpEvent(Vector2 endPoint) {
        if (Mode == ChangeToolMode.Move && _canvas.SelectedShapes.Count > 0 && _firstPoint != endPoint)
            _commandHistory.AddCommand(new MoveCommand(_canvas.SelectedShapes.Last(), endPoint - _firstPoint));
        else if (Mode == ChangeToolMode.Resize && _canvas.SelectedShapes.Count > 0 && _firstPoint != endPoint)
            _commandHistory.AddCommand(new ResizeCommand(_canvas.SelectedShapes.Last(), _firstPoint, endPoint, bbIndex));
        else if (Mode == ChangeToolMode.Rotate && _canvas.SelectedShapes.Count > 0 && _firstPoint != endPoint)
            _commandHistory.AddCommand(new RotateCommand(_canvas.SelectedShapes.Last(), _firstPoint, endPoint));

        _startPoint = Vector2.Zero;
        bbIndex = -1;
        Mode = ChangeToolMode.None;

        if (_isResized) {
            foreach (var shape in _canvas.SelectedShapes)
                shape.NormalizeIndexNodes();
            _isResized = false;
        }
    }





    private int TryGetBBNode(Vector2 point) {
        if (_canvas.SelectedShapes.Count != 1) return -1; // Пока поддерживаем только одну фигуру
        var shape = _canvas.SelectedShapes[0];
        var nodes = shape.BoundingBox;

        //Работает только при условии 1-ой фигуры
        bool isInside = _canvas.IsPointInsideSelectedBB(point); // Точная проверка внутри коробки

        int bbNode = -1;

        for (int i = 0; i < nodes.Count(); i++) {
            float distance = Vector2.Distance(point, nodes[i]);
            if (distance <= NodeSelectionRadius) {
                bbNode = i;
            } else if (!isInside && distance <= RotateActivationRadius) {
                bbNode = 4;
            }
        }
        return bbNode;
    }

    private void SelectMode(Vector2 point) {
        bbIndex = TryGetBBNode(point);
        if (bbIndex >= 0 && bbIndex < 4)
            Mode = ChangeToolMode.Resize; // Изменение размера для узлов 0-3
        else if (bbIndex == 4)
            Mode = ChangeToolMode.Rotate; // Поворот для клика снаружи рядом с углом
        else if (_canvas.IsPointInsideSelectedBB(point))
            Mode = ChangeToolMode.Move;
        else
            Mode = ChangeToolMode.None;
    }

    public void OnChanged() {
        _startPoint = Vector2.Zero;
        _canvas.SelectedShapes.Clear();
        Mode = ChangeToolMode.None;
    }

    public void MouseWheelEvent(float delta, Vector2 currentPoint) {
    }
}

public enum ChangeToolMode {
    None,
    Move,
    Resize,
    Rotate
}