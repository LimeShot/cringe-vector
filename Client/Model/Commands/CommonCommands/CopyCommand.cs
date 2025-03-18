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
    public RelayCommand Command { get; }
    public Vector2 Point { get; set; }
    public string Name { get; private set; }
    public CopyCommand(MyCanvas canvas, Vector2 point, MyCommandHistory commandHistory, List<IShape> shapeBuffer) {
        Name = "Копировать";
        Point = point;
        _canvas = canvas;
        _shapeBuffer = shapeBuffer;
        Command = new(Execute, CanExecute);
    }

    public void Execute() {
        _shapeBuffer.Clear();
        if (_canvas.SelectedShapes.Count == 0) {
            _canvas.SelectShape(Point);
        }
        if (_canvas.IsPointInsideSelectedBB(Point))
            foreach (var shape in _canvas.SelectedShapes) {
                _shapeBuffer.Add(shape);
            }
    }

    public bool CanExecute() => _canvas.Shapes.Count != 0;
}