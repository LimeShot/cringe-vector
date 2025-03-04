using System.Windows;

using CringeCraft.Client.Model.Canvas;

public interface ITool {
    string Name { get; set; }

    void MouseMoveEvent(Point currentPoint);
    void MouseDownEvent(Point startPoint);
    void MouseUpEvent(Point endPoint);
    void OnChanged();
}