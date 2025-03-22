using CringeCraft.Client.Model.Canvas;

using System.Windows;
using System.Windows.Controls;
using OpenTK.Mathematics;
using CringeCraft.Client.Model.Commands.CommandHistory;
using CringeCraft.GeometryDash.Shape;

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
    public List<ICommandMenu> CommandsL = new();
    public Vector2 Point { get; private set; }
    public CommandController(MyCanvas canvas, Camera camera, MyCommandHistory commandHistory) {
        _canvas = canvas;
        _camera = camera;
        _shapeBuffer = new();
        _commandHistory = commandHistory;
        CommandsL.Add(new CopyCommand(_canvas, Point, commandHistory, _shapeBuffer));
        CommandsL.Add(new PasteCommand(_canvas, Point, commandHistory, _shapeBuffer));
        CommandsL.Add(new DelCommand(_canvas, Point, commandHistory));
    }

    public void CreateMenu(Vector2 position) {
        ContextMenu contextMenu = new();
        foreach (var command in CommandsL) {
            contextMenu.Items.Add(new MenuItem {
                Header = command.Name,
                Command = command.Command
            });
            command.Point = position;
        }
        contextMenu.HorizontalOffset = 0;
        contextMenu.VerticalOffset = 0;
        contextMenu.IsOpen = true;
    }
}