namespace CringeCraft.Client.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows;
using CringeCraft.Client.Model;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Render;
using CringeCraft.Client.Model.Tool;
using System.Collections.Specialized;
using CringeCraft.Client.View;
using System.Diagnostics;
using OpenTK.Wpf;
using CringeCraft.Client.Model.Commands;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    private MyCanvas _canvas;

    [ObservableProperty]
    private ToolController _toolController;
    private readonly RenderingService _renderingService;
    private readonly CommandController _commandController;
    private readonly MainWindow _window;
    private readonly Camera _camera;

    public Point StartPoint { get; private set; }
    public Point CurrentPoint { get; private set; }
    public Point CMPosition { get; private set; }

    public MainViewModel(MainWindow window) {
        _window = window;
        _canvas = new();
        _renderingService = new(_canvas);
        _camera = new(_renderingService);
        _commandController = new(_canvas, _camera);
        _toolController = new(_camera, window, _canvas);

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
        _camera.UpdateViewport((float)args.NewSize.Width, (float)args.NewSize.Height);
    }

    public void Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
        _renderingService.Cleanup();
    }

    [RelayCommand]
    private void OnMouseDown(MouseEventArgs e) {
        if (e != null && e.LeftButton == MouseButtonState.Pressed && e.Source is GLWpfControl) { //добавить клик на холст
            StartPoint = e.GetPosition((IInputElement)e.Source);
            _toolController.OnMouseDown(StartPoint);
        }

        if (e != null && e.RightButton == MouseButtonState.Pressed && e.Source is GLWpfControl) {
            CMPosition = e.GetPosition((IInputElement)e.Source);
            _commandController.CreateMenu(CMPosition);
        }
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e) {
        if (e != null && e.Source is GLWpfControl) {
            CurrentPoint = e.GetPosition((IInputElement)e.Source);
            _toolController.OnMouseMove(CurrentPoint, e.LeftButton == MouseButtonState.Pressed);
        }
    }

    [RelayCommand]
    private void OnMouseUp(MouseEventArgs e) {
        if (e != null) {
            _toolController.OnMouseUp(e.GetPosition((IInputElement)e.Source));
        }
    }

    [RelayCommand]
    public void OpenFile() {
        string? content = FileService.OpenFile(Canvas);
        if (content != null) {
            Debug.WriteLine($"Файл открыт:\n{content}");
        }
    }

    [RelayCommand]
    private void SaveFile() {
        string? filePath = FileService.SaveFile(Canvas);
        if (filePath != null) {
            Debug.WriteLine($"Файл сохранён: {filePath}");
        }
    }

}