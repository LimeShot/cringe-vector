using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;
using System.Windows;
using CringeCraft.Client.Model.Canvas;
using CringeCraft.Client.View;


namespace CringeCraft.Client.Model.Tool;

public class ToolController {
    public Dictionary<string, ITool>? Tools = new();
    private readonly MainWindow _window;
    private ToggleButton? _selectedButton;
    private ITool? _currentTool;
    private readonly MyCanvas _canvas;

    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры

    public ToolController(MainWindow window, MyCanvas canvas) {
        _window = window;
        _canvas = canvas;

        //_tools.Add("Change", new ChangeTool());
        foreach (var item in ShapeFactory.AvailableShapes)
            Tools?.Add(item, new CreateTool(item, canvas, OnShapeChanged));

        CreateButtons();
    }

    public void OnMouseDown(Point startPoint) {
        _currentTool?.MouseDownEvent(startPoint);
    }

    public void OnMouseMove(Point currentPoint) {
        _currentTool!.MouseMoveEvent(currentPoint);
    }

    public void OnMouseUp(Point endPoint) {
        _currentTool!.MouseUpEvent(endPoint);
    }

    public void SetTool(string toolName) {
        if (Tools!.TryGetValue(toolName, out var tool)) {
            _currentTool?.OnChanged();
            _currentTool = tool;
        }
    }

    public void CreateButtons() {
        foreach (string typeShape in Tools!.Keys) {
            ToggleButton toggleButton = new() {
                Height = 30,
                Width = 30
            };

            string imagePath = $"pack://siteoforigin:,,,/assets/tools/{typeShape}.png";
            Image toggleImage = new() {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
            };
            toggleButton.Content = toggleImage;

            toggleButton.Checked += (s, e) => {
                if (_selectedButton != null && _selectedButton != toggleButton)
                    _selectedButton.IsChecked = false; // 🔹 Отжимаем предыдущую кнопку
                _selectedButton = toggleButton; // 🔹 Запоминаем новую кнопку

                SetTool(typeShape);
            };
            toggleButton.Unchecked += (s, e) => {
                if (_selectedButton == toggleButton)
                    _selectedButton = null;
            };

            _window.ToolsPanel.Children.Add(toggleButton);
        }
    }
}