using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

namespace CringeCraft.Client.Model.Commands;

public interface ICommand {
    public void Execute();
    public bool CanExecute();
    public void Undo();
    public void Redo();
}