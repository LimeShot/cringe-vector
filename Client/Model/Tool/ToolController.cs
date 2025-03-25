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
using System.Windows.Data;
using System.Globalization;

namespace CringeCraft.Client.Model.Tool;
public partial class ToolController : ObservableObject {
    public Dictionary<string, ITool> Tools = new();
    private readonly MainWindow _window;
    private ToggleButton? _selectedButton;
    private readonly Dictionary<string, ToggleButton> _buttons = new();
    private bool _isCtrlPressed;
    private ITool _currentTool;
    private readonly MyCanvas _canvas;
    private readonly Camera _camera;
    private readonly MyCommandHistory _commandHistory;

    [ObservableProperty]
    private Cursor _currentCursor = Cursors.Arrow;

    [ObservableProperty]
    private bool _isCreateToolActive;

    [ObservableProperty]
    private bool _isPolygonToolActive; // Указывает, активен ли инструмент для многоугольника

    [ObservableProperty]
    private CreateTool? _currentCreateTool;

    public event EventHandler<List<IShape>>? OnShapeChanged;

    public ToolController(Camera camera, MainWindow window, MyCanvas canvas, MyCommandHistory commandHistory) {
        _window = window;
        _canvas = canvas;
        _camera = camera;
        _commandHistory = commandHistory;

        Tools.Add("Change", new ChangeTool("change", canvas, _commandHistory)); //Иконку то пофиксить надо
        Tools.Add("Camera", new CameraTool("change", camera));
        Tools.Add("MoveNode", new MoveNodeTool("movenode", canvas, _commandHistory));

        foreach (var item in ShapeFactory.AvailableShapes)
            Tools.Add(item, new CreateTool(item, canvas, OnShapeChanged, commandHistory));

        _currentTool = Tools["Change"];

        CreateButtons();
    }

    public void OnMouseDown(Vector2 startPoint) {
        _isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        _currentTool.MouseDownEvent(startPoint, _isCtrlPressed);
    }

    public void OnMouseMove(Vector2 currentPoint, bool IsMousePressed) {
        _currentTool.MouseMoveEvent(currentPoint, IsMousePressed);
        UpdateCursor();
        OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }

    public void OnMouseUp(Vector2 endPoint) {
        _currentTool.MouseUpEvent(endPoint);
        OnShapeChanged?.Invoke(this, _canvas.Shapes.ToList());
    }

    public void OnMouseWheel(float delta, Vector2 currentPoint) {
        _currentTool.MouseWheelEvent(delta, currentPoint);
    }

    public void SetTool(string toolName) {
        if (Tools.TryGetValue(toolName, out var tool)) {
            if (_buttons.TryGetValue(toolName, out var targetButton)) {
                if (_selectedButton != targetButton) {
                    if (_selectedButton != null) {
                        _selectedButton.IsChecked = false;
                    }

                    targetButton.IsChecked = true;
                    targetButton.RaiseEvent(new RoutedEventArgs(ToggleButton.CheckedEvent));
                }
            }

            _currentTool?.OnChanged();
            _currentTool = tool;

            if (_currentTool is CreateTool createTool) {
                IsCreateToolActive = true;
                CurrentCreateTool = createTool;
                // Проверяем, является ли инструмент инструментом для многоугольника
                IsPolygonToolActive = createTool.Name.Equals("Polygon", StringComparison.OrdinalIgnoreCase);
            } else {
                IsCreateToolActive = false;
                IsPolygonToolActive = false;
                CurrentCreateTool = null;
            }
        }
    }

    [RelayCommand]
    private void SelectShapeInList(IShape shape) {
        if (!_canvas.SelectedShapes.Contains(shape)) {
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
            var toggleButton = new ToggleButton {
                Height = 30,
                Width = 30
            };

            string imagePath = $"pack://siteoforigin:,,,/assets/tools/{typeTool}.png";
            var toggleImage = new Image {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
            };
            toggleButton.Content = toggleImage;

            toggleButton.Checked += (s, e) => {
                if (_selectedButton != null && _selectedButton != toggleButton)
                    _selectedButton.IsChecked = false;
                _selectedButton = toggleButton;
                SetTool(typeTool);
            };
            toggleButton.Unchecked += (s, e) => {
                if (_selectedButton == toggleButton)
                    _selectedButton = null;
            };
            _window.ToolsPanel.Children.Add(toggleButton);
            _buttons[typeTool] = toggleButton;
        }
    }
}

public class StringToIntConverter : IValueConverter {
    private const int MinSides = 3;
    private const int MaxSides = 20;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value?.ToString() ?? MinSides.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        string? input = value?.ToString();

        // Если строка пустая или null, возвращаем null или текущее значение свойства
        if (string.IsNullOrWhiteSpace(input)) {
            return DependencyProperty.UnsetValue; // Не обновляем источник, оставляем пользователю свободу ввода
        }

        if (int.TryParse(input, out int result)) {
            if (result < MinSides)
                return MinSides;
            if (result > MaxSides)
                return MaxSides;
            return result;
        }

        // Если ввод некорректен (например, буквы), не обновляем источник
        return DependencyProperty.UnsetValue;
    }
}

public class IntRangeValidationRule : ValidationRule {
    private const int MinSides = 3;
    private const int MaxSides = 20;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
        string input = value?.ToString();
        if (string.IsNullOrWhiteSpace(input)) {
            return new ValidationResult(false, "Введите число.");
        }
        if (!int.TryParse(input, out int result)) {
            return new ValidationResult(false, "Должно быть целое число.");
        }
        if (result < MinSides || result > MaxSides) {
            return new ValidationResult(false, $"Число должно быть от {MinSides} до {MaxSides}.");
        }
        return ValidationResult.ValidResult;
    }
}

public class StringToFloatConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value?.ToString() ?? "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if (float.TryParse(value?.ToString(), NumberStyles.Float, culture, out float result)) {
            return result;
        }
        return 0f;
    }
}
