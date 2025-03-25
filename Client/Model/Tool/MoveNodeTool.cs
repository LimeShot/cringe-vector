using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Commands.CommandHistory;
using OpenTK.Mathematics;


public class MoveNodeTool(string name, MyCanvas canvas, MyCommandHistory myCommandHistory) : ITool {
    public string Name => name;

    private readonly MyCanvas _canvas = canvas;
    private bool _isCtrlPressed;
    private readonly MyCommandHistory _myCommandHistory = myCommandHistory;
    private bool _isSelect = false;
    private int _nodeIndex = -1;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private IChangableShape? _shape;
    private const float _eps = 5;


    public void MouseDownEvent(Vector2 startPoint, bool isCtrlPressed) {
        _canvas.CalcTranslate(_canvas.SelectedShapes);
        _startPoint = startPoint;
        _isCtrlPressed = isCtrlPressed;
        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(startPoint);
            if (_canvas.SelectedShapes.Count == 1) {
                _isSelect = true;
                _nodeIndex = GetNodeIndex(startPoint, _canvas.SelectedShapes[0]);
            }
        } else if (_canvas.SelectedShapes.Count == 1) {
            _isSelect = true;
            _nodeIndex = GetNodeIndex(startPoint, _canvas.SelectedShapes[0]);
        }
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (!_isSelect || _nodeIndex == -1) return;
        _canvas.CalcTranslate(_canvas.SelectedShapes);

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