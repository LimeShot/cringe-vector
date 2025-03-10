using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.Model.Commands;
public class PasteCommand : ICommand {
    private readonly RelayCommand _relayCommand;
    private readonly MyCanvas _canvas;
    public string Name { get; private set; }
    public PasteCommand(MyCanvas canvas) {
        Name = "Paste";
        _canvas = canvas;
        _relayCommand = new(Execute, CanExecute);
    }

    public void Execute() {
        //_canvas.Shapes
    }

    public bool CanExecute() => true;

    public void Undo() {
        throw new NotImplementedException();
    }

    public void Redo() {
        throw new NotImplementedException();
    }
}