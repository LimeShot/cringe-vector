using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;
using OpenTK.Mathematics;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
using CringeCraft.Client.Model.Commands.CommandHistory;
public class DelCommand : ICommandMenu {
    private readonly MyCanvas _canvas;
    private readonly MyCommandHistory _commandHistory;
    private const float SelectionRadius = 0.5f;   // Радиус для выбора фигуры
    public RelayCommand Command { get; }
    public string Name { get; private set; }
    public Vector2 Point { get; set; }
    public DelCommand(MyCanvas canvas, Vector2 point, MyCommandHistory commandHistory) {
        Name = "Удалить";
        Point = point;
        _canvas = canvas;
        _commandHistory = commandHistory;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(Point);
        }

        if (_canvas.IsPointInsideSelectedBB(Point)) {
            foreach (var shape in _canvas.SelectedShapes) {
                _canvas.Shapes.Remove(shape);
            }
            _commandHistory.AddCommand(new DeleteCommand(_canvas.SelectedShapes.ToList(), _canvas));
            _canvas.SelectedShapes.Clear();
        }
    }

    public bool CanExecute() => _canvas.Shapes.Count != 0;
}