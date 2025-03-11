using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
public class DelCommand : ICommandMenu {
    private readonly RelayCommand _relayCommand;
    private readonly MyCanvas _canvas;
    public string Name { get; private set; }
    public DelCommand(MyCanvas canvas) {
        Name = "Delete";
        _canvas = canvas;
        _relayCommand = new(Execute, CanExecute);
    }

    public void Execute() {
        //_canvas.Shapes
    }

    public bool CanExecute() => true;
}