using System.Collections.ObjectModel;

using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.Client.Model.Commands.CommandHistory;


public class DeleteAllCommand : ICommand {
    public CommandType Type => CommandType.None;

    private ObservableCollection<IShape> _shapes;
    private readonly List<IShape> _shapesCopy;

    public DeleteAllCommand(ObservableCollection<IShape> shapes) {
        _shapes = shapes;
        _shapesCopy = shapes.ToList();
    }

    public void Redo() {
        _shapes.Clear();
    }

    public void Undo() {
        //_shapes = new ObservableCollection<IShape>(_shapesCopy);
        foreach (IShape shape in _shapesCopy)
            _shapes.Add(shape);
    }
}