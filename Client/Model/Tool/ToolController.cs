namespace CringeCraft.Client.Model.Tool;

using CringeCraft.GeometryDash.Shape;
using CringeCraft.GeometryDash;
using System.Windows.Controls;
using System.Windows;
using CringeCraft.Client.View;
using System.Diagnostics;
using CringeCraft.Client.Model.Canvas;
using System.Numerics;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

public class ToolController {
    private readonly MainWindow _window;
    private readonly MyCanvas _canvas;
    private readonly List<IShape> _selectedShapes = new(); // Список выделенных фигур
    public enum Tools { Move, Rotate, Resize };
    public string CurrentTool { get; private set; }
    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры

    public ToolController(MainWindow window, MyCanvas canvas, string currentTool) {
        _canvas = canvas;
        _window = window;
        CurrentTool = currentTool;
        CreateButtonsShapes();
    }

    public void MoveShape() {
        OnShapeChanged?.Invoke(this, _selectedShapes);
    }

    public void ResizeShape() {
        OnShapeChanged?.Invoke(this, _selectedShapes);
    }

    public void TurnShape() {
        OnShapeChanged?.Invoke(this, _selectedShapes);
    }

    // 🟢 Кнопки для добавления фигуры
    public void CreateButtonsShapes() {
        foreach (string typeShape in ShapeFactory.AvailableShapes) {
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
                CurrentTool = typeShape;
                Debug.WriteLine($"Выбран инструмент - {typeShape}");
            };
            toggleButton.Unchecked += (s, e) => {
                Debug.WriteLine($"Смена инструмента");
            };

            _window.ToolsPanel.Children.Add(toggleButton);
        }
    }

    private void AddShape(MyCanvas canvas, Point startPoint, Point nextPoint) {
        var newShape = ShapeFactory.CreateShape(CurrentTool, new Vector2((float)startPoint.X, (float)startPoint.Y), new Vector2((float)nextPoint.X, (float)nextPoint.Y));
        if (newShape != null)
            canvas.AddShape(newShape);
    }

    public void MouseDownEvent(Point startPoint, Point nextPoint) {
        if (ShapeFactory.AvailableShapes.Contains(CurrentTool))
            AddShape(_canvas, startPoint, nextPoint);
    }

}