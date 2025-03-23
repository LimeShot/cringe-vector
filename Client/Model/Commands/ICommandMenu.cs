namespace CringeCraft.Client.Model.Commands;

using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.Input;

public interface ICommandMenu {
    public Vector2 Point { get; set; }
    public RelayCommand CommandMenu { get; }
    public void ExecuteButton();
}