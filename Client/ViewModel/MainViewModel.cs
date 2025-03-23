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
using VectorPaint;
using CringeCraft.Client.Model.Commands.CommandHistory;
using OpenTK.Graphics.OpenGL;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using OpenTK.Mathematics;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    private MyCanvas _canvas;

    [ObservableProperty]
    private ToolController _toolController;
    private readonly RenderingService _renderingService;
    private readonly CommandController _commandController;
    private readonly MyCommandHistory _commandHistory;
    private readonly MainWindow _window;
    private readonly Camera _camera;
    private RelayCommand? _deleteCommand;
    private RelayCommand? _copyCommand;
    private RelayCommand? _pasteCommand;
    private Vector2 _currentPoint;


    public event Action? CanExecuteChanged;
    public IRelayCommand DeleteCommand => _deleteCommand ??= _commandController.Del;
    public IRelayCommand CopyCommand => _copyCommand ??= _commandController.Copy;
    public IRelayCommand PasteCommand => _pasteCommand ??= _commandController.Paste(CurrentPoint);
    public Vector2 StartPoint { get; private set; }
    public Vector2 CurrentPoint {
        get => _currentPoint;
        set {
            if (_currentPoint != value) {
                _currentPoint = value;
                CanExecuteChanged?.Invoke();
            }
        }
    }
    public Vector2 CMPosition { get; private set; }
    public event EventHandler<List<IShape>>? OnShapeChanged;

    public MainViewModel(MainWindow window) {
        _window = window;
        _canvas = new();
        _renderingService = new(_canvas);
        _camera = new(_renderingService);
        _commandHistory = new MyCommandHistory();
        _commandController = new(_canvas, _camera, _commandHistory);
        _toolController = new(_camera, window, _canvas, _commandHistory);

        _canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции
        _canvas.PropertyChanged += Canvas_SizeChanged;
        _toolController.OnShapeChanged += Shapes_PropertyChanged; // Подписка на изменении фигур
        OnShapeChanged += Shapes_PropertyChanged;
        CanExecuteChanged += PasteCommand_PropertyChanged;
    }

    private void PasteCommand_PropertyChanged() {
        _pasteCommand = _commandController.Paste(CurrentPoint);
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

    private void Canvas_SizeChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(Canvas.Width) || e.PropertyName == nameof(Canvas.Height)) {
            _renderingService.OnCanvasResize(Canvas.Width, Canvas.Height);
        }
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

    public void Closing(object? sender, CancelEventArgs e) {
        _renderingService.Cleanup();
    }

    [RelayCommand]
    private void OnMouseDown(MouseEventArgs e) {
        if (e != null && e.LeftButton == MouseButtonState.Pressed && e.Source is GLWpfControl) { //добавить клик на холст
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            StartPoint = _camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseDown(StartPoint);
        }
        if (e != null && e.RightButton == MouseButtonState.Pressed && e.Source is GLWpfControl) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CMPosition = _camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            _commandController.CreateMenu(CMPosition);
        }
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e) {
        if (e != null && e.Source is GLWpfControl) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CurrentPoint = _camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseMove(CurrentPoint, e.LeftButton == MouseButtonState.Pressed);
        }
    }

    [RelayCommand]
    private void OnMouseUp(MouseEventArgs e) {
        if (e != null) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CurrentPoint = _camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseUp(CurrentPoint);
        }
    }

    [RelayCommand]
    private void MouseWheel(MouseWheelEventArgs e) {
        if (e != null) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CurrentPoint = _camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseWheel(e.Delta, CurrentPoint);
        }
    }

    [RelayCommand]
    public void OpenFile() {
        var (filePath, errorMessage) = FileService.OpenFile(Canvas);
        if (filePath != null) {
            Console.WriteLine($"Открыт файл: {filePath}");
        } else if (errorMessage != null) {
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public void SaveFile() {
        var (filePath, errorMessage) = FileService.SaveFile(Canvas);
        if (filePath != null) {
            Console.WriteLine($"Сохранён файл: {filePath}");
        } else if (errorMessage != null) {
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void ChangeCanvasSize() {
        var resizeCanvasDialog = new ResizeCanvasDialog((int)Canvas.Width, (int)Canvas.Height);
        resizeCanvasDialog.ShowDialog();
        if (resizeCanvasDialog.Tag is bool result && result) {
            if (result == true) {
                Canvas.Width = (float)resizeCanvasDialog.CanvasWidth;
                Canvas.Height = (float)resizeCanvasDialog.CanvasHeight;
            }
        }
    }

    [RelayCommand]
    private void ChangeShapeVisibility(ToggleButton toggleButton) {
        var shape = toggleButton.DataContext as IShape;
        if (shape != null) {
            shape.Style.Visible = !shape.Style.Visible;
            Canvas.SelectedShapes.Remove(shape);
            OnShapeChanged?.Invoke(this, Canvas.Shapes.ToList());
        }
    }

    [RelayCommand]
    private void Undo() {
        if (_commandHistory.UndoCmdCount > 0) {
            var type = _commandHistory.GetLastUndoCommandType();
            _commandHistory.Undo();
            if (type != CommandType.None)
                OnShapeChanged?.Invoke(this, Canvas.Shapes.ToList());
        }
    }

    [RelayCommand]
    private void Redo() {
        if (_commandHistory.RedoCmdCount > 0) {
            var type = _commandHistory.GetLastRedoCommandType();
            _commandHistory.Redo();
            if (type != CommandType.None)
                OnShapeChanged?.Invoke(this, Canvas.Shapes.ToList());
        }
    }
}