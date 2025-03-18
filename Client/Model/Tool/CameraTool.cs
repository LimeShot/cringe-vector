namespace CringeCraft.Client.Model.Tool;

using System.Windows.Input;
using OpenTK.Mathematics;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;

public class CameraTool(string name, Camera camera) : ITool {
    private readonly Camera _camera = camera;
    private Vector2 _startPoint;
    public string Name { get; set; } = name;

    public void MouseDownEvent(Vector2 startPoint) {
        _startPoint = _camera.WorldToScreen(startPoint);
    }

    public void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed) {
        if (isMousePressed) {
            var curr = _camera.WorldToScreen(currentPoint);
            _camera.SetPosition(curr - _startPoint);
            _startPoint = curr;
        }
    }

    public void MouseUpEvent(Vector2 endPoint) {
    }

    public void MouseWheelEvent(float delta, Vector2 currentPoint) {
        _camera.AdjustZoom(delta, _camera.ScreenToWorld(currentPoint));
    }

    public void OnChanged() { return; }
}