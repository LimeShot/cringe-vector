using CommunityToolkit.Mvvm.Input;
using OpenTK.Mathematics;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
public class DelCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    public RelayCommand CommandMenu { get; }
    public Vector2 Point { get; set; }
    public string Prompt { get; private set; }
    public DelCommand(MyCanvas canvas, MyCommandHistory commandHistory) {
        Prompt = "Delete";
        _canvas = canvas;
        _commandHistory = commandHistory;
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
        foreach (var shape in _canvas.SelectedShapes)
            _canvas.RemoveShape(shape);
        _commandHistory.AddCommand(new DeleteCommand(_canvas.SelectedShapes.ToList(), _canvas));
        _canvas.SelectedShapes.Clear();
        _canvas.GetGeneralBB = null;
    }
}