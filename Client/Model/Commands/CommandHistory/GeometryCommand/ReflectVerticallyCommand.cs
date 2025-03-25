using CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class MyReflectVerticallyCommand(IShape shape) : ICommand {
    public CommandType Type => CommandType.Reflect;

    private readonly IShape _shape = shape;

    public void Redo() {
        _shape.ReflectY();
    }

    public void Undo() {
        _shape.ReflectY();
    }
}