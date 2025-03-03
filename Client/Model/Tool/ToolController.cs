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
using OpenTK.Mathematics;

public class ToolController {
    private readonly MainWindow _window;
    private readonly MyCanvas _canvas;
    private ToggleButton? selectedButton; // 🔹 Ссылка на текущую активную кнопку
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
                if (selectedButton != null && selectedButton != toggleButton)
                    selectedButton.IsChecked = false; // 🔹 Отжимаем предыдущую кнопку
                selectedButton = toggleButton; // 🔹 Запоминаем новую кнопку

                CurrentTool = typeShape;
            };
            toggleButton.Unchecked += (s, e) => {
                if (selectedButton == toggleButton)
                    selectedButton = null;
            };

            _window.ToolsPanel.Children.Add(toggleButton);
        }
    }

    private void AddShape(MyCanvas canvas, Point startPoint) {
        var z = canvas.GetNewZ();
        var newShape = ShapeFactory.CreateShape(CurrentTool,
            new OpenTK.Mathematics.Vector2((float)startPoint.X, (float)startPoint.Y), z);
        if (newShape != null)
            canvas.AddShape(newShape);
    }

    public void MouseDownEvent(Point startPoint, Point nextPoint) {
        if (ShapeFactory.AvailableShapes.Contains(CurrentTool))
            AddShape(_canvas, startPoint);
    }

    public void MouseMoveEvent(Point startPoint, Point currentPoint) {
        foreach (IShape item in _canvas.Shapes) {
            if (item.IsBelongsShape(new OpenTK.Mathematics.Vector2((float)currentPoint.X, (float)currentPoint.Y))) {

                int verticeIndex = item.IsBBNode(new OpenTK.Mathematics.Vector2((float)currentPoint.X, (float)currentPoint.Y));

                if (verticeIndex == 0)
                    CurrentTool = Tools.Move.ToString();

                if (verticeIndex >= 1 && verticeIndex <= 4)
                    CurrentTool = Tools.Resize.ToString();

                if (verticeIndex == 5)
                    CurrentTool = Tools.Rotate.ToString();
                break;
            }
        }
    }

}