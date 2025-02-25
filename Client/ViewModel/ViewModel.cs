using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;

using CringeCraft.Client.Model.Canvas;
using System.Printing.IndexedProperties;
using CringeCraft.GeometryDash.Shape;
using System.Collections.ObjectModel;
using CringeCraft.Client.Render;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;


namespace CringeCraft.Client.ViewModel;
public partial class MainViewModel : ObservableObject {

    [ObservableProperty]
    private MyCanvas _canvas;

    [ObservableProperty]
    private ObservableCollection<IShape> _shapes = new ObservableCollection<IShape>();


    public Point StartMousePosition { get; set; }
    public Point NextMousePosition { get; set; }
    private readonly RenderingService _renderingService;
    private readonly Window _window;

    // 🟢 Свойство с автоматическим обновлением UI
    [ObservableProperty]
    private string _statusMessage = "Готово";

    public string? CurrentTool { get; set; }
    private bool _isDragging;

    public MainViewModel(RenderingService renderingService, Window window) {
        _window = window;
        _renderingService = renderingService;

        Canvas = new MyCanvas();
        Canvas.Shapes.CollectionChanged += Shapes_CollectionChanged; // Подписка на изменении коллекции

        // Инициализация ViewModelFigures на основе Canvas.Shapes
        foreach (var shape in Canvas.Shapes) {
            Shapes.Add(shape);
            shape.PropertyChanged += Shapes_PropertyChanged;
        }
    }

    private void Shapes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {
            foreach (IShape newShape in e.NewItems) {
                Shapes.Add(newShape);
                newShape.PropertyChanged += Shapes_PropertyChanged; // Подписка на изменения свойств фигур
            }
        }

        if (e.OldItems != null) {
            foreach (IShape oldShape in e.OldItems) {
                Shapes.Remove(oldShape);
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
        _renderingService.InitializeOpenGL(StatusMessage);
    }

    public void Render(TimeSpan timeSpan) {
        _renderingService.Render(timeSpan);
    }

    public void Resize(object sender, SizeChangedEventArgs args) {
        Console.WriteLine($"Resize: {args.PreviousSize} -> {args.NewSize}");
    }

    // 🟢 Кнопки для добавления фигуры
    private void CreateButtons() {
        foreach (string typeShape in ShapeFabric.AvailableShapes) {
            var button = new Button() {
                Content = typeShape
            };
            button.Click += (s, e) => CurrentTool = typeShape;
        }
    }

    private void CreateTool() {
        Canvas.AddShape(ShapeFabric.CreateShape(CurrentTool, 1, 1));
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