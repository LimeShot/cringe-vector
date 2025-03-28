using OpenTK.Mathematics;

using CringeCraft.GeometryDash.Shape;
using System.Diagnostics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class MoveCommand : ICommand {
    private readonly IShape _shape;
    private readonly Vector2 _delta;

    public CommandType Type => CommandType.Move;
    public MoveCommand(IShape shape, Vector2 delta) {
        _shape = shape;
        _delta = delta;
    }

    public void Undo() {
        _shape.Move(-_delta);
    }

    public void Redo() {
        _shape.Move(_delta);
    }
}