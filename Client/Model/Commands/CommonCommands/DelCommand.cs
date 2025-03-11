using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;
using OpenTK.Mathematics;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
public class DelCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    public RelayCommand Command { get; }
    public string Name { get; private set; }
    public Vector2 Point { get; set; }
    public DelCommand(MyCanvas canvas, Vector2 point) {
        Name = "Удалить";
        Point = point;
        _canvas = canvas;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        foreach (var shape in _canvas.Shapes.Reverse()) {
            if (shape.IsBelongsShape(Point, SelectionRadius)) {
                _canvas.Shapes.Remove(shape);
                break;
            }
        }
    }

    public bool CanExecute() => _canvas.Shapes.Count != 0;
}