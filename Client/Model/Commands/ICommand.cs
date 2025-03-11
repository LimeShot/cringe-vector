namespace CringeCraft.Client.Model.Commands;

public interface ICommand {
    public void Undo();
    public void Redo();
}