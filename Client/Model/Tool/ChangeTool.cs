using System.Windows;

using CringeCraft.Client.Model.Canvas;

namespace CringeCraft.Client.Model.Tool;

public class ChangeTool : ITool {
    public string Name { get; set; }

    public ChangeTool(string name) {
        Name = name;
    }

    public void MouseUpEvent(Point endPoint, MyCanvas canvas) {
        throw new NotImplementedException();
    }

    public void OnChanged() {
        throw new NotImplementedException();
    }

    public void MouseMoveEvent(System.Windows.Point currentPoint) {
        throw new NotImplementedException();
    }

    public void MouseDownEvent(System.Windows.Point startPoint) {
        throw new NotImplementedException();
    }

    public void MouseUpEvent(System.Windows.Point endPoint) {
        throw new NotImplementedException();
    }
}