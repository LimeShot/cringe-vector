using OpenTK.Mathematics;

using CringeCraft.GeometryDash.Shape;
using System.Diagnostics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class ResizeCommand : ICommand {
    private readonly IShape _shape;
    private readonly int _bbIndex;
    private readonly Vector2 _startPoint;
    private readonly Vector2 _endPoint;

    public CommandType Type => CommandType.Resize;
    public ResizeCommand(IShape shape, Vector2 startPoint, Vector2 endPoint, int bbIndex) {
        _shape = shape;
        _startPoint = startPoint;
        _endPoint = endPoint;
        _bbIndex = bbIndex;
    }

    public void Undo() {
        Debug.WriteLine("Вызов Shape.Resize...");
        _shape.Resize(_bbIndex, _startPoint);
        Debug.WriteLine("Shape.Resize отработал");
    }

    public void Redo() {
        _shape.Resize(_bbIndex, _endPoint);
    }
}