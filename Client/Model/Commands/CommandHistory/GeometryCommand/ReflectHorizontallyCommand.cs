using CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class MyReflectHorizontallyCommand(IShape shape) : ICommand {
    public CommandType Type => CommandType.Reflect;

    private readonly IShape _shape = shape;

    public void Redo() {
        _shape.ReflectX();
    }

    public void Undo() {
        _shape.ReflectX();
    }
}