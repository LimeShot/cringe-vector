using System.Windows;

using OpenTK.Mathematics;

public interface ITool {
    string Name { get; }

    void MouseMoveEvent(Vector2 currentPoint, bool isMousePressed);
    void MouseDownEvent(Vector2 startPoint);
    void MouseUpEvent(Vector2 endPoint);
    void OnChanged();
}