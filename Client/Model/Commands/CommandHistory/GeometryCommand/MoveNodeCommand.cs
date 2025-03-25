using System.Windows.Shapes;

using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;

using OpenTK.Mathematics;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class MoveNodeCommand(IChangableShape shape, Vector2 startPoint, Vector2 endPoint, int index) : ICommand {
    public CommandType Type => CommandType.MoveNode;
    private readonly IChangableShape _shape = shape;
    private readonly Vector2 _startPoint = startPoint;
    private readonly Vector2 _endPoint = endPoint;
    private readonly int _index = index;

    public void Redo() {
        _shape.MoveNode(_index, _endPoint);
    }

    public void Undo() {
        _shape.MoveNode(_index, _startPoint);
    }
}