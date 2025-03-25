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
    public RelayCommand CommandMenu { get; }
    public Vector2 Point { get; set; }
    public string Prompt { get; private set; }
    public PasteCommand(MyCanvas canvas, MyCommandHistory commandHistory, List<IShape> shapeBuffer) {
        Prompt = "Ctrl+V";
        _canvas = canvas;
        _shapeBuffer = shapeBuffer;
        _commandHistory = commandHistory;
        CommandMenu = new(ExecuteMenu, CanExecuteMenu);
    }

    private void ExecuteMenu() {
        Execute();
    }

    private bool CanExecuteMenu() => _shapeBuffer.Count != 0;

    public void ExecuteButton() {
        if (_shapeBuffer.Count != 0)
            Execute();
    }

    private void Execute() {
        Vector2 delta;
        _canvas.SelectedShapes.Clear();
        _canvas.CalcTranslate(_shapeBuffer);
        delta = (Point.X - _canvas.GetTranslate.X, Point.Y - _canvas.GetTranslate.Y);
        List<IShape> localShapes = new();
        foreach (var shape in _shapeBuffer) {
            var localShape = shape.Clone();
            localShape.Move(delta);
            localShape.Z = _canvas.GetNewZ();
            localShapes.Add(localShape);
            _canvas.Shapes.Add(localShape);
        }
        _canvas.SelectedShapes.Add(localShapes.Last());
        _commandHistory.AddCommand(new PasteHCommand(localShapes.ToList(), _canvas));
    }
}