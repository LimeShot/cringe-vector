using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class ToBackGroundCommand : ICommand {
    public CommandType Type => CommandType.None;

    private readonly int _prevIndex;
    private readonly MyCanvas _canvas;

    public ToBackGroundCommand(int prevIndex, MyCanvas canvas) {
        _prevIndex = prevIndex;
        _canvas = canvas;
    }

    public void Redo() {
        IShape shape = _canvas.Shapes[_prevIndex];
        _canvas.RemoveShape(shape);
        _canvas.Shapes.Add(shape);
        _canvas.RecalculateAllZ();
    }

    public void Undo() {
        IShape shape = _canvas.Shapes.Last();
        _canvas.RemoveShape(shape);
        _canvas.Shapes.Insert(_prevIndex, shape);
        _canvas.RecalculateAllZ();
    }
}