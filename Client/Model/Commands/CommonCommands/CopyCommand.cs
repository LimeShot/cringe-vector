using System.Diagnostics;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
public class CopyCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    public RelayCommand Command { get; }
    public Vector2 Point { get; set; }
    public string Name { get; private set; }
    public CopyCommand(MyCanvas canvas, Vector2 point) {
        Name = "Копировать";
        Point = point;
        _canvas = canvas;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        // Debug.WriteLine("Копируем..йоууу");
    }

    public bool CanExecute() => true;
}