using System.Runtime.InteropServices;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class PasteCommand : ICommand {
    private readonly List<IShape> _pastedShapes;
    private readonly MyCanvas _canvas;

    public CommandType Type => CommandType.None;

    public PasteCommand(List<IShape> pastedShapes, MyCanvas canvas) {
        _pastedShapes = pastedShapes;
        _canvas = canvas;
    }

    public void Redo() {
        foreach (IShape shape in _pastedShapes) {
            _canvas.AddShape(shape);
        }
    }

    public void Undo() {
        foreach (IShape shape in _pastedShapes) {
            _canvas.RemoveShape(shape);
        }
    }
}