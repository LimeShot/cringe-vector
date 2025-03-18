using OpenTK.Mathematics;

using CringeCraft.GeometryDash.Shape;
using System.Diagnostics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class RotateCommand : ICommand {
    private readonly IShape _shape;
    private readonly Vector2 _startPoint;
    private readonly Vector2 _endPoint;

    public RotateCommand(IShape shape, Vector2 startPoint, Vector2 endPoint) {
        _shape = shape;
        _startPoint = startPoint;
        _endPoint = endPoint;
    }

    public void Undo() {
        Debug.WriteLine("Вызов Shape.Rotate...");
        _shape.RotateShape(_endPoint, _startPoint);
        Debug.WriteLine("Shape.Rotate отработал");
    }

    public void Redo() {
        _shape.RotateShape(_startPoint, _endPoint);
    }
}