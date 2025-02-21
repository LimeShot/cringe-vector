using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Tools;

namespace ViewModelMV;
public partial class ViewModel : ObservableObject {

    private readonly ToolController _toolController;
    
    [RelayCommand]
    private void ClickTool(string tool) {
        _toolController.ChangeTool(tool);
    }

    public ViewModel() {
        _toolController = new ToolController();
    }
}