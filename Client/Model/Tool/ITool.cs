using System.Windows;

using OpenTK.Mathematics;

public interface ITool {
    string Name { get; }

    void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed);
    void MouseDownEvent(Vector2 startPoint, bool isCtrlPressed);
    void MouseUpEvent(Vector2 endPoint);
    void MouseWheelEvent(float delta, Vector2 currentPoint);
    void OnChanged();
}