using System.Windows;
using System.Diagnostics;
using OpenTK.Mathematics;

using CringeCraft.Client.Model.Canvas;
using System.Printing.IndexedProperties;

namespace CringeCraft.Client.Model.Tool;

public class ChangeTool(string name, MyCanvas canvas) : ITool {
    private readonly MyCanvas _canvas = canvas;
    private Vector2 _startPoint;
    private int _bbindex = -1;
    private bool _move = false;
    private bool _resize = false;
    private bool _rotate = false;


    public string Name { get; set; } = name;

    public void OnChanged() {
        _startPoint = Vector2.Zero;
        _canvas.SelectedShapes.Clear();
    }

    public void MouseMoveEvent(Vector2 currentPoint) {
        if (_canvas.SelectedShapes.Count != 0) {
            if (_move) {
                foreach (var shape in _canvas.SelectedShapes) {
                    var delta = currentPoint - _startPoint;
                    shape.Move(delta);
                    _startPoint = currentPoint;
                }
            } else if (_resize) {
                foreach (var shape in _canvas.SelectedShapes) {
                    shape.Resize(_bbindex, currentPoint);
                }
            } else if (_rotate) {
                foreach (var shape in _canvas.SelectedShapes) {
                    //shape.Rotate(_bbindex, currentPoint);
                }
            }
        }
    }

    public void MouseDownEvent(Vector2 startPoint) {
        bool unselect = false;
        if (_canvas.SelectedShapes.Count == 0) {
            SelectShape(startPoint, 1.0f);
        } else {
            _bbindex = TryGetBBNode(startPoint, 5.0f);  //Магический 3.0f)
            if (_bbindex < 4 && _bbindex > -1) {
                _resize = true;
                _rotate = false;
                _move = false;
                return;
            } else if (_bbindex == -1) {
                foreach (var shape in _canvas.SelectedShapes) {
                    if (shape.IsBelongsShape(startPoint, 1.0f)) { //Магический 3.0f)
                        _resize = false;
                        _rotate = false;
                        _move = true;
                        _startPoint = startPoint;
                        return;
                    }
                }
                unselect = true;
            }
        }
        if (unselect) {
            _canvas.SelectedShapes.Clear();
        }
        Debug.WriteLine($"Фигур выбрано: {_canvas.SelectedShapes.Count}");
        Debug.WriteLine($"move: {_move}");
        Debug.WriteLine($"resize: {_resize}");
    }

    public void MouseUpEvent(Vector2 endPoint) {
    }

    private void SelectShape(Vector2 point, float radiusPoint) {
        var index = _canvas.Shapes.Count - 1;
        foreach (var shape in _canvas.Shapes.Reverse()) {
            if (shape.IsBelongsShape(point, radiusPoint)) { //Магический 3.0f)
                _canvas.SelectedShapes.Add(_canvas.Shapes[index]);
                return;
            }
            index--;
        }
    }

    private int TryGetBBNode(Vector2 point, float radiusPoint) { //Работает при условии, что в Selected Shape один элемент
        int bbNode = -1;
        float minDistance = 52;

        foreach (var shape in _canvas.SelectedShapes) {
            for (int i = 0; i < shape.BoundingBox.Length; i++) {
                float distance = Vector2.Distance(point, shape.BoundingBox[i]);
                if (distance <= radiusPoint && distance < minDistance) {
                    minDistance = distance;
                    bbNode = i;
                }
            }
        }
        return bbNode;
    }
}