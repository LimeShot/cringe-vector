using OpenTK.Mathematics;

using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.GeometryCommands;

public class RotateCommand : ICommand {
    private readonly IShape _shape;
    private readonly Vector2 _startPoint;
    private readonly Vector2 _endPoint;

    public RotateCommand(IShape shape, Vector2 startPoint, Vector2 endPoint) {
        _shape = shape;
        _startPoint = startPoint;
        _endPoint = endPoint;
    }

    public void Redo() {
        _shape.RotateShape(_endPoint, _startPoint);
    }

    public void Undo() {
        _shape.RotateShape(_startPoint, _endPoint);
    }
}