using System.Diagnostics;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
public class PasteCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    public RelayCommand Command { get; }
    public string Name { get; private set; }
    public Vector2 Point { get; set; }
    public PasteCommand(MyCanvas canvas, Vector2 point, MyCommandHistory commandHistory) {
        Name = "Вставить";
        Point = point;
        _canvas = canvas;
        _commandHistory = commandHistory;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        // Debug.WriteLine("Вставляем..йоуу");
    }

    public bool CanExecute() => true;
}