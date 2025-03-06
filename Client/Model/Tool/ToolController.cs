using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using OpenTK.Mathematics;

using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.View;


namespace CringeCraft.Client.Model.Tool;

public class ToolController {
    public Dictionary<string, ITool> Tools = new();
    private readonly MainWindow _window;
    private ToggleButton? _selectedButton;
    private ITool _currentTool;
    private readonly MyCanvas _canvas;

    private double _actualWidth;
    private double _actualHeight;

    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры

    public ToolController(MainWindow window, MyCanvas canvas) {
        _window = window;
        _canvas = canvas;

        _actualWidth = window.ActualWidth;
        _actualHeight = window.ActualHeight;

        Tools.Add("Change", new ChangeTool("change", canvas)); //Иконку то пофиксить надо

        foreach (var item in ShapeFactory.AvailableShapes)
            Tools.Add(item, new CreateTool(item, canvas, OnShapeChanged));

        _currentTool = Tools["Change"];

        CreateButtons();
    }

    public void UpdateSize() {
        _actualWidth = _window.OpenTkControl.ActualWidth;
        _actualHeight = _window.OpenTkControl.ActualHeight;
    }

    public void OnMouseDown(Point startPoint) {
        UpdateSize();
        startPoint = new Point(startPoint.X - _actualWidth / 2, -startPoint.Y + _actualHeight / 2);
        _currentTool.MouseDownEvent(new Vector2((float)startPoint.X, (float)startPoint.Y));
    }

    public void OnMouseMove(Point currentPoint) {
        currentPoint = new Point(currentPoint.X - _actualWidth / 2, -currentPoint.Y + _actualHeight / 2);
        _currentTool.MouseMoveEvent(new Vector2((float)currentPoint.X, (float)currentPoint.Y));
        OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }

    public void OnMouseUp(Point endPoint) {
        endPoint = new Point(endPoint.X - _actualWidth / 2, -endPoint.Y + _actualHeight / 2);
        _currentTool.MouseUpEvent(new Vector2((float)endPoint.X, (float)endPoint.Y));
        OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }

    public void SetTool(string toolName) {
        if (Tools.TryGetValue(toolName, out var tool)) {
            _currentTool.OnChanged();
            _currentTool = tool;
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