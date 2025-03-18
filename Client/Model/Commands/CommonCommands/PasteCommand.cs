using System.Diagnostics;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CringeCraft.GeometryDash.Shape;
using System.Runtime.InteropServices.Marshalling;
using System.IO.Compression;
public class PasteCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    private readonly List<IShape> _shapeBuffer;
    public RelayCommand Command { get; }
    public string Name { get; private set; }
    public Vector2 Point { get; set; }
    public PasteCommand(MyCanvas canvas, Vector2 point, MyCommandHistory commandHistory, List<IShape> shapeBuffer) {
        Name = "Вставить";
        Point = point;
        _canvas = canvas;
        _shapeBuffer = shapeBuffer;
        _commandHistory = commandHistory;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        Vector2 delta;
        if (_shapeBuffer.Count == 1) {
            delta = Point - _shapeBuffer[0].Translate;
        } else {
            Vector2 center = _canvas.GetCenterOfGBB(_shapeBuffer);
            delta = new(Point.X - center.X, Point.Y - center.Y);
        }
        List<IShape> localShapes = new();
        foreach (var shape in _shapeBuffer) {
            var localShape = shape.Clone();
            localShape.Move(delta);
            localShape.Z = _canvas.GetNewZ();
            localShapes.Add(localShape);
            _canvas.Shapes.Add(localShape);
        }
        _commandHistory.AddCommand(new PasteHCommand(localShapes.ToList(), _canvas));
    }

    public bool CanExecute() => _shapeBuffer.Count != 0;
}