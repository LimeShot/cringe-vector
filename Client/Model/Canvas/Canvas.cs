using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Canvas;
/// <summary>
/// Представляет холст для хранения фигур
/// </summary>
public class Canvas {
    public readonly List<IShape> Shapes;

    public Canvas() {
        Shapes = new List<IShape>();
    }

    public void AddShape(IShape shape) {
        Shapes.Add(shape);
    }

    public void RemoveShape(IShape shape) {
        Shapes.Remove(shape);
    }

    public void SaveCanvas() {
        // pass
    }
}