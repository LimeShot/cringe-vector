using CringeCraft.Client.Model.Canvas;

using System.Windows;
using System.Windows.Controls;
using OpenTK.Mathematics;
using CringeCraft.Client.Model.Commands.CommandHistory;

namespace CringeCraft.Client.Model.Commands;

public class CommandController {
    private readonly MyCanvas _canvas;
    private readonly Camera _camera;
    private readonly MyCommandHistory _commandHistory;

    public List<ICommandMenu> CommandsL = new();
    public Vector2 Point { get; private set; }
    public CommandController(MyCanvas canvas, Camera camera, MyCommandHistory commandHistory) {
        _canvas = canvas;
        _camera = camera;
        _commandHistory = commandHistory;
        CommandsL.Add(new CopyCommand(_canvas, Point));
        CommandsL.Add(new PasteCommand(_canvas, Point));
        CommandsL.Add(new DelCommand(_canvas, Point));
    }

    public void CreateMenu(Point position) {
        ContextMenu contextMenu = new();
        Point = _camera.ScreenToWorld(new Vector2((float)position.X, (float)position.Y));
        foreach (var command in CommandsL) {
            contextMenu.Items.Add(new MenuItem {
                Header = command.Name,
                Command = command.Command
            });
            command.Point = Point;
        }
        contextMenu.HorizontalOffset = 0;
        contextMenu.VerticalOffset = 0;
        contextMenu.IsOpen = true;
    }
}