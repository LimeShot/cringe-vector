using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.GeometryCommands;

public class CreateCommand : ICommand {
    private readonly IShape _shape;
    private readonly ICanvas _canvas;

    public CreateCommand(IShape shape, ICanvas canvas) {
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