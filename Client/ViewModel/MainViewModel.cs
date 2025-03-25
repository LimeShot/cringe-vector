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
using System.Windows.Media;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    private MyCanvas _canvas;

    [ObservableProperty]
    private ToolController _toolController;
    private bool _isPopupOpen = false;
    private readonly RenderingService _renderingService;
    private readonly CommandController _commandController;
    private readonly MyCommandHistory _commandHistory;
    private readonly MainWindow _window;

    [ObservableProperty]
    private Camera _camera;

    public Vector2 StartPoint { get; private set; }
    public Vector2 CurrentPoint { get; set; }
    public Vector2 CMPosition { get; private set; }

    public event EventHandler<List<IShape>>? OnShapeChanged;

    public MainViewModel(MainWindow window) {
        _window = window;
        _commandHistory = new MyCommandHistory();
        _canvas = new(_commandHistory);
        _renderingService = new(_canvas);
        Camera = new(_renderingService);
        _commandController = new(_canvas, Camera, _commandHistory);
        _toolController = new(Camera, window, _canvas, _commandHistory);

        _canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции
        _canvas.PropertyChanged += Canvas_SizeChanged;
        _canvas.PropertyChanged += BoundingBoxChanged;
        _canvas.OnShapeChanged += Shapes_PropertyChanged;
        _toolController.OnShapeChanged += Shapes_PropertyChanged; // Подписка на изменении фигур
        OnShapeChanged += Shapes_PropertyChanged;
        PopupStateManager.PopupStateChanged += state => _isPopupOpen = state;
    }

    private void Shapes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {
            var newShapes = e.NewItems.Cast<IShape>().ToArray();
            _renderingService.OnShapeAdded(newShapes);

            MoveShapeUpCommand.NotifyCanExecuteChanged();
            MoveShapeDownCommand.NotifyCanExecuteChanged();
        }
        if (e.OldItems != null) {
            var oldShapes = e.OldItems.Cast<IShape>().ToArray();
            _renderingService.OnShapeRemoved(oldShapes);

            MoveShapeUpCommand.NotifyCanExecuteChanged();
            MoveShapeDownCommand.NotifyCanExecuteChanged();
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

    private void BoundingBoxChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(Canvas.GetGeneralBB)) {
            _renderingService.OnBoundingBoxChanged(Canvas.GetGeneralBB);
        }
    }

    public void InitializeOpenGL() {
        _renderingService.Initialize();
    }

    public void Render(TimeSpan timeSpan) {
        _renderingService.Render(timeSpan);
    }

    public void Resize(object sender, SizeChangedEventArgs args) {
        Camera.UpdateViewport((float)args.NewSize.Width, (float)args.NewSize.Height);
    }

    public void Closing(object? sender, CancelEventArgs e) {
        _renderingService.Cleanup();
    }

    [RelayCommand]
    private void OnCtrlPressed() {

    }

    [RelayCommand]
    private void OnCtrlReleased() {

    }

    [RelayCommand]
    private void OnMouseDown(MouseEventArgs e) {
        if (e != null && !_isPopupOpen && e.LeftButton == MouseButtonState.Pressed && e.Source is GLWpfControl) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            StartPoint = Camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseDown(StartPoint);
        } else if (e != null && e.RightButton == MouseButtonState.Pressed && e.Source is GLWpfControl) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CMPosition = Camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            _commandController.CreateMenu(CMPosition);
        }
    }

    [RelayCommand]
    private void OnMouseMove(MouseEventArgs e) {
        if (e != null && !_isPopupOpen && e.Source is GLWpfControl) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CurrentPoint = Camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseMove(CurrentPoint, e.LeftButton == MouseButtonState.Pressed);
        }
    }

    [RelayCommand]
    private void OnMouseUp(MouseEventArgs e) {
        if (e != null) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CurrentPoint = Camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseUp(CurrentPoint);
        }
    }

    [RelayCommand]
    private void MouseWheel(MouseWheelEventArgs e) {
        if (e != null) {
            var screenPoint = e.GetPosition((IInputElement)e.Source);
            CurrentPoint = Camera.ScreenToWorld(new Vector2((float)screenPoint.X, (float)screenPoint.Y));
            ToolController.OnMouseWheel(e.Delta, CurrentPoint);
        }
    }

    [RelayCommand]
    public void OpenFile() {
        var (filePath, errorMessage) = FileService.OpenFile(Canvas);
        if (filePath != null) {
            Console.WriteLine($"Открыт файл: {filePath}");
            Canvas.Shapes.CollectionChanged += Shapes_CollectionChanged;
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
                Canvas.Width = resizeCanvasDialog.CanvasWidth;
                Canvas.Height = resizeCanvasDialog.CanvasHeight;
                Canvas.CanvasSize = $"Холст: {Canvas.Width}x{Canvas.Height}";
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
            if (type != CommandType.None) {
                OnShapeChanged?.Invoke(this, Canvas.Shapes.ToList());
                Canvas.CalcTranslate(Canvas.SelectedShapes);
            }
        }
    }

    [RelayCommand]
    private void Redo() {
        if (_commandHistory.RedoCmdCount > 0) {
            var type = _commandHistory.GetLastRedoCommandType();
            _commandHistory.Redo();
            if (type != CommandType.None) {
                OnShapeChanged?.Invoke(this, Canvas.Shapes.ToList());
                Canvas.CalcTranslate(Canvas.SelectedShapes);
            }
        }
    }

    [RelayCommand]
    private void ChangeOutLineColor(Color? color) {
        if (color.HasValue) {
            Canvas.ChangeOutLineColor(color.Value);
        }
    }

    [RelayCommand]
    private void ChangeFillColor(Color? color) {
        if (color.HasValue) {
            Canvas.ChangeFillColor(color.Value);
        }
    }

    [RelayCommand(CanExecute = nameof(CanMoveShapeUp))]
    private void MoveShapeUp(IShape shape) {
        Canvas.MoveShapeUp(shape);
    }

    private bool CanMoveShapeUp(IShape shape) =>
        shape != null && Canvas.Shapes.IndexOf(shape) > 0;

    [RelayCommand(CanExecute = nameof(CanMoveShapeDown))]
    private void MoveShapeDown(IShape shape) {
        Canvas.MoveShapeDown(shape);
    }

    private bool CanMoveShapeDown(IShape shape) =>
        shape != null && Canvas.Shapes.IndexOf(shape) < Canvas.Shapes.Count - 1;

    [RelayCommand]
    private void Paste() {
        var command = _commandController.CommandsL["Вставить"];
        command.Point = CurrentPoint;
        command.ExecuteButton();
    }

    [RelayCommand]
    private void Copy() {
        _commandController.CommandsL["Копировать"].ExecuteButton();
    }

    [RelayCommand]
    private void Delete() {
        _commandController.CommandsL["Удалить"].ExecuteButton();
    }

    [RelayCommand]
    private void DeleteAllShapes() {
        Canvas.DeleteAllShapes();
        OnShapeChanged?.Invoke(this, Canvas.Shapes.ToList());
    }

    [RelayCommand]
    private void TopMenuOpened() {
        PopupStateManager.NotifyPopUpOpened();
    }

    [RelayCommand]
    private void TopMenuClosed() {
        PopupStateManager.NotifyPopUpClosed();
    }

    [RelayCommand]
    private void BringToBack() {
        _commandController.CommandsL["На задний план"].ExecuteButton();
    }

    [RelayCommand]
    private void BringToFront() {
        _commandController.CommandsL["На передний план"].ExecuteButton();
    }

    [RelayCommand]
    private void ReflectVertically() {
        _commandController.CommandsL["Отразить по вертикали"].ExecuteButton();
    }

    [RelayCommand]
    private void ReflectHorizontally() {
        _commandController.CommandsL["Отразить по горизонтали"].ExecuteButton();
    }

    [RelayCommand]
    private void SelectAll() {
        Canvas.SelectedShapes = Canvas.Shapes.ToList();
        Canvas.CalcTranslate(Canvas.SelectedShapes);
    }
}

public static class PopupStateManager {
    public static event Action<bool>? PopupStateChanged;

    public static void NotifyPopUpOpened() {
        PopupStateChanged?.Invoke(true);
    }

    public static void NotifyPopUpClosed() {
        PopupStateChanged?.Invoke(false);
    }
}

