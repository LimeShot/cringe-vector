namespace CringeCraft.Client.Model.Commands;

using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

public interface ICommandMenu {
    public string Name { get; }
    public Vector2 Point { get; set; }
    public RelayCommand CommandMenu { get; }
    public RelayCommand CommandButton { get; }
}