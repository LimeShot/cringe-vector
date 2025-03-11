using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.View;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
namespace CringeCraft.Client.Model.Commands;

public class CommandController {
    private readonly MyCanvas _canvas;
    public Dictionary<string, ICommandMenu> Commands = new();
    public CommandController(MyCanvas canvas) {
        _canvas = canvas;
        // foreach (string item in Enum.GetNames<CommandNames>()) {
        //     Commands.Add(item, new CopyCommand(_canvas));
        // }
        Commands.Add("Копировать", new CopyCommand(_canvas));
        Commands.Add("Вставить", new PasteCommand(_canvas));
        Commands.Add("Удалить", new DelCommand(_canvas));
    }

    public void CreateMenu(Point position) {
        ContextMenu contextMenu = new() {
            Items = {
                new MenuItem {Header = "Копировать"},
                new MenuItem {Header = "Вставить"},
                new MenuItem {Header = "Удалить"}
            }
        };
        contextMenu.HorizontalOffset = position.X;
        contextMenu.VerticalOffset = position.Y;
        contextMenu.IsOpen = true;
    }

    // private enum CommandNames { Copy, Paste, Delete };
    // private ObservableCollection<string> _items;
}