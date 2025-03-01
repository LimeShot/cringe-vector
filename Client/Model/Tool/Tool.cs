namespace CringeCraft.Client.Model.Tool;

using CringeCraft.GeometryDash.Shape;
using CringeCraft.GeometryDash;
using System.Windows.Controls;
using System.Windows;
using CringeCraft.Client.View;
using System.Diagnostics;
using CringeCraft.Client.Model.Canvas;
using System.Numerics;

public class Tool {
    private readonly MainWindow _window;
    private readonly MyCanvas _canvas;
    private readonly List<IShape> _selectedShapes = new(); // Список выделенных фигур

    public string CurrentTool { get; set; }
    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры

    public Tool(MainWindow window, MyCanvas canvas) {
        _canvas = canvas;
        _window = window;
        CurrentTool = "Line";
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
        StackPanel stackPanel = new() {
            Orientation = Orientation.Vertical, // Расположение кнопок в столбик
            HorizontalAlignment = HorizontalAlignment.Right, // Выравнивание по правому краю
            VerticalAlignment = VerticalAlignment.Top, // Выравнивание по верхнему краю
            Margin = new(10) // Отступ от краёв
        };

        _window.MainGrid.Children.Add(stackPanel);

        foreach (string typeShape in ShapeFactory.AvailableShapes) {
            Button button = new() {
                Content = typeShape,
                Width = 100,
                Margin = new(5),
            };
            stackPanel.Children.Add(button);
            // На клик привязываем команду для смены инструмента
            button.Click += (s, e) => {
                CurrentTool = typeShape;
                Debug.WriteLine($"Выбран инструмент - {typeShape}");
            };
        }
    }

    private void AddShape(MyCanvas canvas, Point startPoint, Point nextPoint) {
        var newShape = ShapeFactory.CreateShape(CurrentTool, new Vector2((float)startPoint.X, (float)startPoint.Y), new Vector2((float)nextPoint.X, (float)nextPoint.Y));
        if (newShape != null)
            canvas.AddShape(newShape);
    }
}