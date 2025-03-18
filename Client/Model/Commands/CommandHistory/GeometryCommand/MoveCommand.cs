using OpenTK.Mathematics;

using CringeCraft.GeometryDash.Shape;
using System.Diagnostics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class MoveCommand : ICommand {
    private readonly IShape _shape;
    private readonly Vector2 _delta;

    public MoveCommand(IShape shape, Vector2 delta) {
        _shape = shape;
        _delta = delta;
    }

    public void Undo() {
        Debug.WriteLine("Вызов Shape.Move...");
        _shape.Move(-_delta);
        Debug.WriteLine("Shape.Move отработал");
    }

    public void Redo() {
        _shape.Move(_delta);
    }
}