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
    public RelayCommand CommandButton { get; }
    public Vector2 Point { get; set; }
    public string Name { get; private set; }
    public CopyCommand(MyCanvas canvas, Vector2 point, MyCommandHistory commandHistory, List<IShape> shapeBuffer) {
        Name = "Копировать";
        Point = point;
        _canvas = canvas;
        _shapeBuffer = shapeBuffer;
        CommandMenu = new(ExecuteMenu, CanExecuteMenu);
        CommandButton = new(ExecuteButton, CanExecuteButton);
    }

    private void ExecuteMenu() {
        _shapeBuffer.Clear();
        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(Point);
        }
        if (_canvas.IsPointInsideSelectedBB(Point))
            foreach (var shape in _canvas.SelectedShapes) {
                _shapeBuffer.Add(shape);
            }
    }

    private bool CanExecuteMenu() => _canvas.Shapes.Count != 0;

    private void ExecuteButton() {
        _shapeBuffer.Clear();
        foreach (var shape in _canvas.SelectedShapes)
            _shapeBuffer.Add(shape);
    }

    private bool CanExecuteButton() => _canvas.SelectedShapes.Count != 0;
}