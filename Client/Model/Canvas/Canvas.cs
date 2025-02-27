using CringeCraft.GeometryDash.Shape;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CringeCraft.GeometryDash.Canvas;

namespace CringeCraft.Client.Model.Canvas;
/// <summary>
/// Представляет холст для хранения фигур
/// </summary>
public partial class MyCanvas : ObservableObject, ICanvas {

    [ObservableProperty]
    private ObservableCollection<IShape> _shapes = new ObservableCollection<IShape>();
    public float LengthCanvas { get; set; }
    public float WeightCanvas { get; set; }
    public MyCanvas() {
        LengthCanvas = 500;
        WeightCanvas = 500;
    }

    public void AddShape(IShape shape) {
        Shapes.Add(shape);
    }

    public void RemoveShape(IShape shape) {
        Shapes.Remove(shape);
    }

    public void SaveCanvas() {
        // pass
    }
}