namespace CringeCraft.Client.Model.Commands;

public interface ICommandMenu {
    public string Name { get; }
    public void Execute();
    public bool CanExecute();
}