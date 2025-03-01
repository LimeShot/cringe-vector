namespace CringeCraft.Client.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Render;
using CringeCraft.Client.Model.Tool;
using System.Collections.Specialized;
using CringeCraft.Client.View;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    private MyCanvas _canvas;

    private Tool _tool;
    private readonly RenderingService _renderingService;
    private readonly MainWindow _window;
    private bool _isDragging;

    public Point StartMousePosition { get; set; }
    public Point NextMousePosition { get; set; }

    public MainViewModel(MainWindow window) {
        _window = window;
        _canvas = new();
        _tool = new(window, _canvas);
        _renderingService = new(_canvas);

        _canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции
        _tool.OnShapeChanged += Shapes_PropertyChanged; // Подписка на изменении фигур
    }

    private void Shapes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {
            var newShapes = e.NewItems.Cast<IShape>().ToArray();
            _renderingService.OnShapeAdded(newShapes);
        }
        if (e.OldItems != null) {
            var oldShapes = e.OldItems.Cast<IShape>().ToArray();
            _renderingService.OnShapeRemoved(oldShapes);
        }
    }

    private void Shapes_PropertyChanged(object? sender, List<IShape> selectedShapes) {
        _renderingService.OnShapeUpdated([.. selectedShapes]);
    }

    public void InitializeOpenGL() {
        _renderingService.Initialize();
    }

    public void Render(TimeSpan timeSpan) {
        _renderingService.Render(timeSpan);
    }

    public void Resize(object sender, SizeChangedEventArgs args) {
        _renderingService.OnResize((int)args.NewSize.Width, (int)args.NewSize.Height);
    }

    public void Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
        _renderingService.Cleanup();
    }

    [RelayCommand]
    private void OnMouseDown(MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            StartMousePosition = e.GetPosition((IInputElement)e.Source);
            _isDragging = true;
            if (e.Source is UIElement element)
                element.CaptureMouse();
        }
    }

    [RelayCommand]
    private void OnMouseMove(MouseButtonEventArgs e) {
        if (_isDragging == true)
            NextMousePosition = e.GetPosition((IInputElement)e.Source);
    }

    [RelayCommand]
    private void OnMouseUp(MouseButtonEventArgs e) { }
}