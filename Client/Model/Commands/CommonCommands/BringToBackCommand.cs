using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands.CommandHistory;

using OpenTK.Mathematics;

namespace CringeCraft.Client.Model.Commands;

public class BringToBackCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;

    public Vector2 Point { get; set; }
    public RelayCommand CommandMenu { get; }

    public BringToBackCommand(MyCanvas canvas, MyCommandHistory commandHistory) {
        _canvas = canvas;
        _commandHistory = commandHistory;
        CommandMenu = new(ExecuteMenu, CanExecuteMenu);
    }

    private void ExecuteMenu() {
        Execute();
    }

    private bool CanExecuteMenu() => _canvas.SelectedShapes.Count == 1;

    public void ExecuteButton() {
        if (_canvas.SelectedShapes.Count == 1)
            Execute();
    }

    private void Execute() {
        var shape = _canvas.SelectedShapes.First();
        _canvas.RemoveShape(shape);
        _canvas.Shapes.Add(shape);
        _canvas.RecalculateAllZ();
    }
}