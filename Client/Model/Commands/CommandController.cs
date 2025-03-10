using CringeCraft.Client.Model.Canvas;

using System.Collections.ObjectModel;

using System.Windows.Controls;
namespace CringeCraft.Client.Model.Commands;

public class CommandController {
    private readonly MyCanvas _canvas;
    public Dictionary<string, ICommand> Commands = new();
    public CommandController(MyCanvas canvas) {
        _canvas = canvas;
        foreach (string item in Enum.GetNames<CommandNames>()) {
            Commands.Add(item, new CopyCommand(_canvas));
        }
        // CreateMenu();
    }

    public void CreateMenu() {
        foreach (string item in Enum.GetNames<CommandNames>()) {

        }
    }

    private enum CommandNames { Copy, Paste, Delete };
    // private ObservableCollection<string> _items;
}