using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CringeCraft.GeometryDash.Shape;


public class ReflectVerticallyCommand : ICommandMenu {
    public Vector2 Point { get; set; }
    public string Prompt { get; private set; }
    public RelayCommand CommandMenu { get; }

    private MyCanvas _canvas;
    private MyCommandHistory _commandHistory;

    public ReflectVerticallyCommand(MyCanvas canvas, MyCommandHistory commandHistory) {
        Prompt = "Ctrl+Alt+V";
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
        var shape = _canvas.SelectedShapes[0];
        shape.ReflectY();
        _commandHistory.AddCommand(new MyReflectVerticallyCommand(shape));
        _canvas.CringeEvent();
        _canvas.CalcTranslate(_canvas.SelectedShapes);
    }
}