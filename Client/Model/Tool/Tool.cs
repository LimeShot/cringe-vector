using System.Windows.Xps.Serialization;

using CringeCraft.GeometryDash.Shape;
using CringeCraft.GeometryDash;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CringeCraft.Client.View;
using System.Diagnostics;
using CringeCraft.Client.Model.Canvas;
using System.Numerics;

namespace CringeCraft.Client.Model.Tool;

public class Tool {
    private readonly MainWindow _window;
    private readonly MyCanvas _canvas;
    public string CurrentTool { get; set; }
    public event EventHandler<List<IShape>>? OnShapeChanged; // Событие на изменение фигуры
    private readonly List<IShape> _selectedShapes; // Список выделенных фигур


    public Tool(MainWindow window, MyCanvas canvas) {
        _canvas = canvas;
        _window = window;
        CurrentTool = "Line";
        _selectedShapes = new List<IShape>();
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
        var stackPanel = new StackPanel() {
            Orientation = Orientation.Vertical, // Расположение кнопок в столбик
            HorizontalAlignment = HorizontalAlignment.Right, // Выравнивание по правому краю
            VerticalAlignment = VerticalAlignment.Top, // Выравнивание по верхнему краю
            Margin = new Thickness(10) // Отступ от краёв
        };

        _window.MainGrid.Children.Add(stackPanel);

        foreach (string typeShape in ShapeFactory.AvailableShapes) {
            var button = new Button() {
                Content = typeShape,
                Width = 100,
                Margin = new Thickness(5)
            };
            stackPanel.Children.Add(button);
            button.Click += (s, e) => {
                CurrentTool = typeShape;
                Debug.WriteLine($"Выбран инструмент - {typeShape}");
            }; // На клик привязываем команду для смены инструмента
        }
    }

    private void AddShape(MyCanvas canvas, Point startPoint, Point nextPoint) {
        var newShape = ShapeFactory.CreateShape(CurrentTool, new Vector2((float)startPoint.X,
            (float)startPoint.Y), new Vector2((float)nextPoint.X, (float)nextPoint.Y));
        if (newShape != null) {
            canvas.AddShape(newShape);
        }
    }
}