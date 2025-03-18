using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class CreateCommand : ICommand {
    private readonly IShape _shape;
    private readonly ICanvas _canvas;

    public CommandType Type => CommandType.None;
    public CreateCommand(IShape shape, ICanvas canvas) {
        _shape = shape;
        _canvas = canvas;
    }

    public void Undo() {
        _canvas.RemoveShape(_shape);
    }

    public void Redo() {
        _canvas.AddShape(_shape);
    }
}