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
using System.Windows.Shapes;


namespace CringeCraft.Client.ViewModel;
public partial class MainViewModel : ObservableObject {

    [ObservableProperty]
    private MyCanvas _canvas;
    public Point StartMousePosition { get; set; }
    public Point NextMousePosition { get; set; }
    private readonly RenderingService _renderingService;
    private readonly Window _window;

    public string CurrentTool { get; set; }
    private bool _isDragging;

    public MainViewModel(Window window) {
        _window = window;

        CurrentTool = "GOOOOL";

        Canvas = new MyCanvas();

        Canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции

        _renderingService = new(Canvas);

        // Инициализация ViewModelFigures на основе Canvas.Shapes
        foreach (var shape in Canvas.Shapes) {
            shape.PropertyChanged += Shapes_PropertyChanged;
        }
    }

    private void Shapes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {
            foreach (IShape newShape in e.NewItems) {
                newShape.PropertyChanged += Shapes_PropertyChanged; // Подписка на изменения свойств фигур
            }
        }

        if (e.OldItems != null) {
            foreach (IShape oldShape in e.OldItems) {
                oldShape.PropertyChanged -= Shapes_PropertyChanged; // Отписка при удалении
            }
        }

        // Что-то сделать при изменении списка фигур
    }

    private void Shapes_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        // Обработка изменений свойств фигуры
        if (sender is IShape changedFigure) {
            // Что-то сделать при изменении свойст фигуры
        }
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
            button.Click += (s, e) => CurrentTool = typeShape;
        }
    }

    private void CreateTool() {
        var newShape = ShapeFactory.CreateShape(CurrentTool, 1, 1);
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