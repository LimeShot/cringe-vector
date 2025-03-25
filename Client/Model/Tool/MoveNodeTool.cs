namespace CringeCraft.Client.Model.Tool;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Commands.CommandHistory;
using OpenTK.Mathematics;


public class MoveNodeTool : ITool {
    public string Name { get; }

    private readonly MyCanvas _canvas;
    private bool _isCtrlPressed;
    private readonly MyCommandHistory _myCommandHistory;
    private bool _isSelect = false;
    private int _nodeIndex = -1;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private IChangableShape? _shape;
    private const float _eps = 5;

    public event Action? OnBoundingBoxChanged;

    public MoveNodeTool(string name, MyCanvas canvas, MyCommandHistory myCommandHistory) {
        Name = name;
        _canvas = canvas;
        _myCommandHistory = myCommandHistory;
        OnBoundingBoxChanged += canvas.OnCringeBoundingBoxChanged;
    }


    public void MouseDownEvent(Vector2 startPoint, bool isCtrlPressed) {
        _startPoint = startPoint;
        _isCtrlPressed = isCtrlPressed;
        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(startPoint);
            if (_canvas.SelectedShapes.Count == 1) {
                _isSelect = true;
                _nodeIndex = GetNodeIndex(startPoint, _canvas.SelectedShapes[0]);
            }
        } else if (_canvas.SelectedShapes.Count == 1) {
            if (_canvas.IsPointInsideSelectedBB(startPoint)) {
                _isSelect = true;
                _nodeIndex = GetNodeIndex(startPoint, _canvas.SelectedShapes[0]);
            } else {
                _canvas.SelectedShapes.Clear();
                OnBoundingBoxChanged?.Invoke();
            }
        }
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (!_isSelect || _nodeIndex == -1) return;
        OnBoundingBoxChanged?.Invoke();

        if (_canvas.SelectedShapes[0] is IChangableShape) {
            _shape = (IChangableShape)_canvas.SelectedShapes[0];
            _shape.MoveNode(_nodeIndex, currentPoint);
            _endPoint = currentPoint;
        }
    }

    public void MouseUpEvent(Vector2 endPoint) {
        _isSelect = false;
        if (_nodeIndex != -1)
            _myCommandHistory.AddCommand(new MoveNodeCommand(_shape, _startPoint, _endPoint, _nodeIndex));
    }

    private int GetNodeIndex(Vector2 point, IShape shape) {
        Matrix2.CreateRotation(MathHelper.DegreesToRadians(-shape.Rotate), out Matrix2 result);
        var localPoint = result * (point - shape.Translate);

        for (int i = 0; i < shape.Nodes.Length; i++) {
            if ((shape.Nodes[i] - localPoint).Length < _eps) {
                _nodeIndex = i;
                break;
            }
            _nodeIndex = -1;
        }

        return _nodeIndex;
    }

    public void MouseWheelEvent(float delta, Vector2 currentPoint) {
        return;
    }

    public void OnChanged() {
        return;
    }
}