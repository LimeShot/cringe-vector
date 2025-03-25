using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class DeleteCommand : ICommand {
    private readonly MyCanvas _canvas;
    private readonly List<IShape> _deletedShapes;

    public CommandType Type => CommandType.None;


    public DeleteCommand(List<IShape> shapes, MyCanvas canvas) {
        _deletedShapes = shapes;
        _canvas = canvas;
    }

    public void Redo() {
        foreach (IShape shape in _deletedShapes) {
            _canvas.RemoveShape(shape);
        }
    }

    public void Undo() {
        foreach (IShape shape in _deletedShapes) {
            _canvas.AddShape(shape);
        }
    }
}