using System.Diagnostics;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
public class CopyCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    public RelayCommand Command { get; }
    public Vector2 Point { get; set; }
    public string Name { get; private set; }
    public CopyCommand(MyCanvas canvas, Vector2 point, MyCommandHistory commandHistory) {
        Name = "Копировать";
        Point = point;
        _canvas = canvas;
        _commandHistory = commandHistory;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        // Debug.WriteLine("Копируем..йоууу");
    }

    public bool CanExecute() => true;
}