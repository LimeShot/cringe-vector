namespace CringeCraft.Client.Model.Commands.CommandHistory;

public interface ICommand {
    public void Undo();
    public void Redo();
}