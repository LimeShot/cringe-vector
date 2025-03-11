using OpenTK.Mathematics;

using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.GeometryCommands;

public class ResizeCommand : ICommand {
    private readonly IShape _shape;
    private readonly int _bbIndex;
    private readonly Vector2 _startPoint;
    private readonly Vector2 _endPoint;

    public ResizeCommand(IShape shape) {
        _shape = shape;
    }

    public void Redo() {
        _shape.Resize(_bbIndex, _startPoint);
    }

    public void Undo() {
        _shape.Resize(_bbIndex, _endPoint);
    }
}