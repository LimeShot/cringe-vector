using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Render;
using CringeCraft.Client.Model.Tool;


namespace CringeCraft.Client.ViewModel;
public partial class MainViewModel : ObservableObject {

    [ObservableProperty]
    private MyCanvas _canvas;

    private readonly Tool _tool;

    public Point StartMousePosition { get; set; }
    public Point NextMousePosition { get; set; }
    private readonly RenderingService _renderingService;
    private readonly Window _window;
    private bool _isDragging;

    public MainViewModel(Window window) {
        _window = window;

        Canvas = new MyCanvas();

        _tool = new Tool();

        Canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции

        _tool.OnShapeChanged += Shapes_PropertyChanged; // Подписка на изменении фигур

        _renderingService = new(Canvas);
    }

    private void Shapes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {

            // var newShapes = e.NewItems.Cast<IShape>().ToList();
            // _renderingService.OnShapeAdded(newShapes);

            foreach (IShape newShape in e.NewItems) {
                _renderingService.OnShapeAdded(newShape); // Обработка новых фигур
            }
        }

        if (e.OldItems != null) {

            // var oldShapes = e.OldItems.Cast<IShape>().ToList();
            // _renderingService.OnShapeRemoved(newShapes);

            foreach (IShape oldShape in e.OldItems) {
                _renderingService.OnShapeRemoved(oldShape); // Обработка удаленных фигур
            }
        }
    }

    private void Shapes_PropertyChanged(object? sender, List<IShape> selectedShapes) {
        // _renderingService.OnShapeUpdated(selectedShapes);
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

    // 🟢 Кнопки для добавления фигуры
    private void CreateButtons() {
        foreach (string typeShape in ShapeFactory.AvailableShapes) {
            var button = new Button() {
                Content = typeShape
            };
            button.Click += (s, e) => _tool.CurrentTool = typeShape;
        }
    }

    private void CreateTool() {
        var newShape = ShapeFactory.CreateShape(_tool.CurrentTool, 1, 1);
        if (newShape != null) {
            Canvas.AddShape(newShape);
        }
    }

    [RelayCommand]
    private void OnMouseDown(MouseButtonEventArgs e) {
        if (e.LeftButton == MouseButtonState.Pressed) {
            StartMousePosition = e.GetPosition((IInputElement)e.Source);
            _isDragging = true;
            if (e.Source is UIElement element) {
                element.CaptureMouse();
            }
        }
    }
    [RelayCommand]
    private void OnMouseMove(MouseButtonEventArgs e) {
        if (_isDragging == true) {
            NextMousePosition = e.GetPosition((IInputElement)e.Source);

        }
    }
    [RelayCommand]
    private void OnMouseUp(MouseButtonEventArgs e) {

    }
}