namespace CringeCraft.Client.Model.Commands;

public interface ICommand {
    public string Name { get; }
    public void Execute();
    public bool CanExecute();
    public void Undo();
    public void Redo();
}