using CringeCraft.Client.Model.Canvas;

using System.Windows;
using System.Windows.Controls;
using OpenTK.Mathematics;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CringeCraft.GeometryDash.Shape;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace CringeCraft.Client.Model.Commands;

public struct CopyBuffer {
    public List<IShape> ShapeBuffer;
    public Vector2[] BoundingBox;
    public Vector2 OriginPoint;
}

public class CommandController {
    private readonly MyCanvas _canvas;
    private readonly Camera _camera;
    private readonly MyCommandHistory _commandHistory;
    private readonly List<IShape> _shapeBuffer;
    public Dictionary<string, ICommandMenu> CommandsL = new();
    public CommandController(MyCanvas canvas, Camera camera, MyCommandHistory commandHistory) {
        _canvas = canvas;
        _camera = camera;
        _shapeBuffer = new();
        _commandHistory = commandHistory;
        CommandsL.Add("Копировать", new CopyCommand(_canvas, _commandHistory, _shapeBuffer));
        CommandsL.Add("Вставить", new PasteCommand(_canvas, _commandHistory, _shapeBuffer));
        CommandsL.Add("Удалить", new DelCommand(_canvas, _commandHistory));
        CommandsL.Add("На передний план", new BringToFrontCommand(_canvas, _commandHistory));
        CommandsL.Add("На задний план", new BringToBackCommand(_canvas, _commandHistory));
    }

    public void CreateMenu(Vector2 position) {
        ContextMenu contextMenu = new();
        if (!_canvas.IsPointInsideSelectedBB(position)) {
            _canvas.SelectedShapes.Clear();
            _canvas.SelectShape(position);
        }
        foreach (var command in CommandsL) {
            contextMenu.Items.Add(new MenuItem {
                Header = command.Key,
                Command = command.Value.CommandMenu
            });
            command.Value.Point = position;
        }
        contextMenu.HorizontalOffset = 0;
        contextMenu.VerticalOffset = 0;
        contextMenu.IsOpen = true;
    }
}