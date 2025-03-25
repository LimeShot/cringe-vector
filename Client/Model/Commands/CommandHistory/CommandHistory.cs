namespace CringeCraft.Client.Model.Commands.CommandHistory;

public class MyCommandHistory {
    private const int CommandsCount = 50;
    private readonly LinkedList<ICommand> _commands = new LinkedList<ICommand>();
    private readonly LinkedList<ICommand> _cancelledCommands = new LinkedList<ICommand>();

    public int UndoCmdCount { get; private set; } = 0;
    public int RedoCmdCount { get; private set; } = 0;

    public void AddCommand(ICommand command) {
        if (_commands.Count >= CommandsCount) {
            _commands.RemoveLast();
            UndoCmdCount -= 1;
        }

        _commands.AddFirst(command);
        _cancelledCommands.Clear();
        UndoCmdCount += 1;
        RedoCmdCount = 0;
    }


    public void Undo() {
        if (_commands.Count > 0) {
            var removeCommand = _commands.First!.Value;
            removeCommand.Undo();

            _commands.RemoveFirst();
            _cancelledCommands.AddFirst(removeCommand);
            UndoCmdCount -= 1;
            RedoCmdCount += 1;
        }
    }

    public void Redo() {
        if (_cancelledCommands.Count > 0) {
            var command = _cancelledCommands.First!.Value;
            command.Redo();

            _cancelledCommands.RemoveFirst();
            _commands.AddFirst(command);
            UndoCmdCount += 1;
            RedoCmdCount -= 1;
        }
    }

    public CommandType GetLastUndoCommandType() {
        return _commands.First().Type;
    }
    public CommandType GetLastRedoCommandType() {
        return _cancelledCommands.First().Type;
    }
}