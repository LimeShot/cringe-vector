namespace CringeCraft.Client.Model.Commands.CommandHistory;

public interface ICommand {
    public CommandType Type { get; }
    public void Undo();
    public void Redo();
}

public enum CommandType {
    None,
    Move,
    Resize,
    Rotate,
    MoveNode,
    Reflect
}