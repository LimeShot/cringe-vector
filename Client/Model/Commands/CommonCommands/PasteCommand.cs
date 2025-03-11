using System.Diagnostics;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
public class PasteCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    public RelayCommand Command { get; }
    public string Name { get; private set; }
    public Vector2 Point { get; set; }
    public PasteCommand(MyCanvas canvas, Vector2 point) {
        Name = "Вставить";
        Point = point;
        _canvas = canvas;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        // Debug.WriteLine("Вставляем..йоуу");
    }

    public bool CanExecute() => true;
}