namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class CmdHistory {
    private const int CommandsCount = 20;
    private readonly LinkedList<ICommand> _commands = new LinkedList<ICommand>();
    private readonly LinkedList<ICommand> _cancelledCommands = new LinkedList<ICommand>();

    public void AddCommand(ICommand command) {
        if (_commands.Count >= CommandsCount)
            _commands.RemoveLast();

        _commands.AddFirst(command);
        _cancelledCommands.Clear();
    }


    public void Undo() {
        if (_commands.Count > 0) {
            var removeCommand = _commands.First!.Value;
            removeCommand.Undo();

            _commands.RemoveFirst();
            _cancelledCommands.AddFirst(removeCommand);
        }
    }

    public void Redo() {
        if (_cancelledCommands.Count > 0) {
            var command = _cancelledCommands.First!.Value;
            command.Redo();

            _cancelledCommands.RemoveFirst();
            AddCommand(command);
        }
    }
}