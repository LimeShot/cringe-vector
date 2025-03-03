namespace CringeCraft.Client.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Render;
using CringeCraft.Client.Model.Tool;
using System.Collections.Specialized;
using CringeCraft.Client.View;
using CringeCraft.GeometryDash;
using System.Diagnostics;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    private MyCanvas _canvas;

    private readonly ToolController _toolController;
    private readonly RenderingService _renderingService;
    private readonly MainWindow _window;
    private bool isDrawing = false;

    public Point StartPoint { get; set; }
    public Point CurrentPoint { get; set; }

    public MainViewModel(MainWindow window) {
        _window = window;
        _canvas = new();
        _toolController = new(window, _canvas, "Line");
        _renderingService = new(_canvas);

        _canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции
        _toolController.OnShapeChanged += Shapes_PropertyChanged; // Подписка на изменении фигур
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
    private void OnMouseDown(MouseEventArgs e) {
        if (e != null && e.LeftButton == MouseButtonState.Pressed) { //добавить клик на холст
            //&& e.Source not is подложка под кнопочки clickedподложка под кнопочки
            isDrawing = true;
            StartPoint = e.GetPosition((IInputElement)e.Source);
            Debug.WriteLine($"Мышка нажата: X={StartPoint.X}, Y={StartPoint.Y}");
            //_toolController.MouseDownEvent(StartPoint, StartPoint);
        }
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e) {
        if (e != null && isDrawing == true) {
            CurrentPoint = e.GetPosition((IInputElement)e.Source);
            Debug.WriteLine($"Мышка зажата: X={CurrentPoint.X}, Y={CurrentPoint.Y}");
        }
    }

    [RelayCommand]
    private void OnMouseUp(MouseEventArgs e) {
        if (e != null)
            isDrawing = false;
    }
}