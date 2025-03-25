using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class ToForeGroundCommand : ICommand {
    public CommandType Type => CommandType.None;

    private readonly int _prevIndex;
    private readonly MyCanvas _canvas;

    public ToForeGroundCommand(int prevIndex, MyCanvas canvas) {
        _prevIndex = prevIndex;
        _canvas = canvas;
    }

    public void Redo() {
        IShape shape = _canvas.Shapes[_prevIndex];
        _canvas.RemoveShape(shape);
        _canvas.AddShape(shape);
        _canvas.RecalculateAllZ();
    }

    public void Undo() {
        IShape shape = _canvas.Shapes[0];
        _canvas.RemoveShape(shape);
        _canvas.Shapes.Insert(_prevIndex, shape);
        _canvas.RecalculateAllZ();
    }
}