namespace CringeCraft.Client.Model.Tool;

using OpenTK.Mathematics;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands.CommandHistory;
using System.Dynamic;

public class ChangeTool : ITool {
    private const float NodeSelectionRadius = 4.0f;   // Радиус для активации изменения размера
    private const float RotateActivationRadius = 10.0f; // Радиус для активации поворота

    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    private Vector2 _startPoint;
    private Vector2 _firstPoint;
    private bool _isSeveralShapes = false;
    public int bbIndex { get; private set; } = -1;
    public ChangeToolMode Mode { get; private set; } = ChangeToolMode.None;
    private bool _isResized = false;
    public string Name { get; }

    public event Action? OnBoundingBoxChanged;

    public ChangeTool(string name, MyCanvas canvas, MyCommandHistory commandHistory) {
        Name = name;
        _canvas = canvas;
        _commandHistory = commandHistory;
        OnBoundingBoxChanged += canvas.OnCringeBoundingBoxChanged;
    }

    public void MouseDownEvent(Vector2 startPoint, bool isCtrlPressed) {
        _startPoint = startPoint;
        _firstPoint = startPoint;

        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(startPoint);
            OnBoundingBoxChanged?.Invoke();

            if (_canvas.SelectedShapes.Count > 0)
                Mode = ChangeToolMode.Move;

        } else {
            SelectMode(startPoint);

            if (Mode == ChangeToolMode.None) {
                if (!isCtrlPressed)
                    _canvas.SelectedShapes.Clear();

                _canvas.SelectShape(startPoint);
                OnBoundingBoxChanged?.Invoke();

                if (_canvas.SelectedShapes.Count > 0)
                    Mode = ChangeToolMode.Move;
                else
                    _canvas.GetGeneralBB = null;
            }
        }
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (_canvas.SelectedShapes.Count == 0) return;

        if (_canvas.SelectedShapes.Count > 1) _isSeveralShapes = true;
        else _isSeveralShapes = false;

        if (isMousePressed) {
            OnBoundingBoxChanged?.Invoke();
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
            _commandHistory.AddCommand(new MoveCommand(_canvas.SelectedShapes[0], endPoint - _firstPoint));
        else if (Mode == ChangeToolMode.Resize && _canvas.SelectedShapes.Count > 0 && _firstPoint != endPoint)
            _commandHistory.AddCommand(new ResizeCommand(_canvas.SelectedShapes[0], _firstPoint, endPoint, bbIndex));
        else if (Mode == ChangeToolMode.Rotate && _canvas.SelectedShapes.Count > 0 && _firstPoint != endPoint)
            _commandHistory.AddCommand(new RotateCommand(_canvas.SelectedShapes[0], _firstPoint, endPoint));

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
        _canvas.CalcTranslate(_canvas.SelectedShapes);
        bool isInside = _canvas.IsPointInsideSelectedBB(point);

        int bbNode = -1;

        for (int i = 0; i < _canvas.GetGeneralBB.Count(); i++) {
            float distance = Vector2.Distance(point, _canvas.GetGeneralBB[i]);
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
        if (bbIndex >= 0 && bbIndex < 4 && !_isSeveralShapes)
            Mode = ChangeToolMode.Resize; // Изменение размера для узлов 0-3
        else if (bbIndex == 4 && !_isSeveralShapes)
            Mode = ChangeToolMode.Rotate; // Поворот для клика снаружи рядом с углом
        else if (_canvas.IsPointInsideSelectedBB(point))
            Mode = ChangeToolMode.Move;
        else
            Mode = ChangeToolMode.None;
    }

    public void OnChanged() {
        _startPoint = Vector2.Zero;
        _canvas.SelectedShapes.Clear();
        _canvas.GetGeneralBB = null;
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