using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using OpenTK.Mathematics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.View;
using CringeCraft.Client.Model.Commands.CommandHistory;

namespace CringeCraft.Client.Model.Tool;

public partial class ToolController : ObservableObject {
    public Dictionary<string, ITool> Tools = new();
    private readonly MainWindow _window;
    private ToggleButton? _selectedButton;
    private ITool _currentTool;
    private readonly MyCanvas _canvas;
    private readonly Camera _camera;
    private readonly MyCommandHistory _commandHistory;

    [ObservableProperty]
    private Cursor _currentCursor = Cursors.Arrow;

    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры

    public ToolController(Camera camera, MainWindow window, MyCanvas canvas, MyCommandHistory commandHistory) {
        _window = window;
        _canvas = canvas;
        _camera = camera;
        _commandHistory = commandHistory;

        Tools.Add("Change", new ChangeTool("change", canvas, _commandHistory)); //Иконку то пофиксить надо
        Tools.Add("Camera", new CameraTool("change", camera));

        foreach (var item in ShapeFactory.AvailableShapes)
            Tools.Add(item, new CreateTool(item, canvas, OnShapeChanged, commandHistory));

        _currentTool = Tools["Change"];

        CreateButtons();
    }

    public void OnMouseDown(Point startPoint) {
        var screenPoint = _camera.ScreenToWorld(new Vector2((float)startPoint.X, (float)startPoint.Y));
        _currentTool.MouseDownEvent(screenPoint);
    }

    public void OnMouseMove(Point currentPoint, bool IsMousePressed) {
        var screenPoint = _camera.ScreenToWorld(new Vector2((float)currentPoint.X, (float)currentPoint.Y));
        _currentTool.MouseMoveEvent(screenPoint, IsMousePressed);
        UpdateCursor();
        OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }

    public void OnMouseUp(Point endPoint) {
        var screenPoint = _camera.ScreenToWorld(new Vector2((float)endPoint.X, (float)endPoint.Y));
        _currentTool.MouseUpEvent(screenPoint);
        OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }

    public void SetTool(string toolName) {
        if (Tools.TryGetValue(toolName, out var tool)) {
            _currentTool.OnChanged();
            _currentTool = tool;
        }
    }

    [RelayCommand]
    private void SelectShapeInList(IShape shape) {
        if (!_canvas.SelectedShapes.Contains(shape)) {
            //Надо бы зажать кнопку Change
            SetTool("Change");
            _canvas.SelectedShapes.Clear();
            _canvas.SelectedShapes.Add(shape);
        }
    }

    private void UpdateCursor() {
        if (_currentTool is ChangeTool changeTool) {
            CurrentCursor = changeTool.Mode switch {
                ChangeToolMode.Resize => changeTool.bbIndex switch {
                    0 => Cursors.SizeNWSE,
                    1 => Cursors.SizeNESW,
                    2 => Cursors.SizeNWSE,
                    3 => Cursors.SizeNESW,
                    _ => Cursors.Arrow
                },
                ChangeToolMode.Rotate => Cursors.Cross,
                ChangeToolMode.Move => Cursors.SizeAll,
                _ => Cursors.Arrow
            };
        } else {
            CurrentCursor = Cursors.Arrow;
        }
    }

    public void CreateButtons() {
        foreach (string typeTool in Tools!.Keys) {
            ToggleButton toggleButton = new() {
                Height = 30,
                Width = 30
            };

            string imagePath = $"pack://siteoforigin:,,,/assets/tools/{typeTool}.png";
            Image toggleImage = new() {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
            };
            toggleButton.Content = toggleImage;

            toggleButton.Checked += (s, e) => {
                if (_selectedButton != null && _selectedButton != toggleButton)
                    _selectedButton.IsChecked = false; // 🔹 Отжимаем предыдущую кнопку
                _selectedButton = toggleButton; // 🔹 Запоминаем новую кнопку

                SetTool(typeTool);
            };
            toggleButton.Unchecked += (s, e) => {
                if (_selectedButton == toggleButton)
                    _selectedButton = null;
            };

            _window.ToolsPanel.Children.Add(toggleButton);
        }
    }
}