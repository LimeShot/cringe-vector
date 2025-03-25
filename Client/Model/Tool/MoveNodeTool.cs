using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Commands.CommandHistory;
using OpenTK.Mathematics;


public class MoveNodeTool(string name, MyCanvas canvas) : ITool {
    public string Name => name;

    private readonly MyCanvas _canvas = canvas;
    private bool _isSelect = false;
    private int _nodeIndex = -1;
    private const float _eps = 3;


    public void MouseDownEvent(Vector2 startPoint) {

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

        if (_canvas.SelectedShapes[0] is IChangableShape) {
            IChangableShape shape = (IChangableShape)_canvas.SelectedShapes[0];
            shape.MoveNode(_nodeIndex, currentPoint);
        }
    }

    public void MouseUpEvent(Vector2 endPoint) {
        _isSelect = false;
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