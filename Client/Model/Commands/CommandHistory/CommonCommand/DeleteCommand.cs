using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class DeleteCommand : ICommand {
    private readonly MyCanvas _canvas;
    private readonly IShape _shape;

    public CommandType Type => CommandType.None;


    public DeleteCommand(IShape shape, MyCanvas canvas) {
        _shape = shape;
        _canvas = canvas;
    }

    public void Redo() {
        _canvas.RemoveShape(_shape);
    }

    public void Undo() {
        _canvas.AddShape(_shape);
    }
}