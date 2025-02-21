using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.Client.Model.Tools;
using CringeCraft.Client.Model.Canvas;
using System.Printing.IndexedProperties;
using CringeCraft.GeometryDash.Shape;
using System.Collections.ObjectModel;
using CringeCraft.Client.Render;


namespace CringeCraft.Client.ViewModel;
public partial class MainViewModel : ObservableObject {

    private readonly ToolController _toolController;

    private readonly Canvas _canvas;

    private readonly RenderingService _renderingService;

    [ObservableProperty]
    private ObservableCollection<IShape> _shapes;

    // 🟢 Свойство с автоматическим обновлением UI
    [ObservableProperty]
    private string _statusMessage = "Готово";
    
    [RelayCommand]
    private void ClickTool(string tool) {
        _toolController.ChangeTool(tool);
    }

    public MainViewModel(RenderingService renderingService) {
        _toolController = new ToolController();
        _renderingService = renderingService;
        _canvas = new Canvas();
        _shapes = new ObservableCollection<IShape>(_canvas.Shapes);
    }

    public void InitializeOpenGL() {
        _renderingService.InitializeOpenGL(StatusMessage);
    }
    
    public void Render(TimeSpan timeSpan) {
        _renderingService.Render(timeSpan);
    }
}