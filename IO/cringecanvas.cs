using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CringeCraft.GeometryDash;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.IO;
public partial class CringeCanvas : ObservableObject, ICanvas {
    [ObservableProperty]
    private ObservableCollection<IShape> _shapes = new();

    private const float _stepZ = 0.0000001f;

    public readonly List<IShape> SelectedShapes = new(); // Список выделенных фигур

    public float Width { get; set; }
    public float Height { get; set; }
    public float ScreenPerWorld = 1.0f;

    public CringeCanvas() {
        Width = 2000;
        Height = 1000;
    }

    private void recalculateZFromRemove(int index) {
        for (int i = index; i < Shapes.Count; i++) {
            Shapes[i].Z -= _stepZ;
        }
    }

    public float GetNewZ() {
        return Shapes.Count * _stepZ;
    }

    public void AddShape(IShape shape) {
        Shapes.Add(shape);
    }

    public void RemoveShape(IShape shape) {
        int index = Shapes.IndexOf(shape);
        Shapes.Remove(shape);
        recalculateZFromRemove(index);
    }

    public void SaveCanvas() {
        //
    }
}