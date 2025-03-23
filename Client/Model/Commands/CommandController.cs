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
    public Vector2[] BountedBox;
    public Vector2 OriginPoint;
}

public class CommandController {
    private readonly MyCanvas _canvas;
    private readonly Camera _camera;
    private readonly MyCommandHistory _commandHistory;
    private readonly List<IShape> _shapeBuffer;
    public Dictionary<string, ICommandMenu> CommandsL = new();
    public Vector2 Point { get; private set; }
    public CommandController(MyCanvas canvas, Camera camera, MyCommandHistory commandHistory) {
        _canvas = canvas;
        _camera = camera;
        _shapeBuffer = new();
        _commandHistory = commandHistory;
        CommandsL.Add("Копировать", new CopyCommand(_canvas, Point, commandHistory, _shapeBuffer));
        CommandsL.Add("Вставить", new PasteCommand(_canvas, Point, commandHistory, _shapeBuffer));
        CommandsL.Add("Удалить", new DelCommand(_canvas, Point, commandHistory));
    }

    public void CreateMenu(Vector2 position) {
        ContextMenu contextMenu = new();
        if (_canvas.SelectedShapes.Count == 0) {
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

    public RelayCommand Del => CommandsL["Удалить"].CommandButton;

    public RelayCommand Copy => CommandsL["Копировать"].CommandButton;

    public RelayCommand Paste(Vector2 point) {
        var command = CommandsL["Вставить"];
        command.Point = point;
        return command.CommandButton;
    }
}