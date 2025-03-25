using System.Diagnostics;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;
// using Newtonsoft.Json;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CringeCraft.GeometryDash.Shape;

public class CopyCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly List<IShape> _shapeBuffer;
    public RelayCommand CommandMenu { get; }
    public Vector2 Point { get; set; }
    public string Prompt { get; private set; }
    public CopyCommand(MyCanvas canvas, MyCommandHistory commandHistory, List<IShape> shapeBuffer) {
        Prompt = "Ctrl+C";
        _canvas = canvas;
        _shapeBuffer = shapeBuffer;
        CommandMenu = new(ExecuteMenu, CanExecuteMenu);
    }

    private void ExecuteMenu() {
        Execute();
    }

    private bool CanExecuteMenu() => _canvas.SelectedShapes.Count != 0;

    public void ExecuteButton() {
        if (_canvas.SelectedShapes.Count != 0)
            Execute();
    }

    private void Execute() {
        _shapeBuffer.Clear();
        foreach (var shape in _canvas.SelectedShapes)
            _shapeBuffer.Add(shape);
    }
}