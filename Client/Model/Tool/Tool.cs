using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Tool;

public class Tool {

    public string CurrentTool { get; set; }
    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры
    private readonly List<IShape> _selectedShapes; // Список выделенных фигур

    public Tool() {
        CurrentTool = "Line";
        _selectedShapes = new List<IShape>();
    }

    public void MoveShape() {
        OnShapeChanged?.Invoke(this, _selectedShapes);
    }

    public void ResizeShape() {
        OnShapeChanged?.Invoke(this, _selectedShapes);
    }
}