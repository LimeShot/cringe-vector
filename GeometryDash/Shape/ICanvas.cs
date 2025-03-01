namespace CringeCraft.GeometryDash.Canvas;

using System.ComponentModel;
using System.Collections.ObjectModel;
using CringeCraft.GeometryDash.Shape;

public interface ICanvas : INotifyPropertyChanged {
   public ObservableCollection<IShape> Shapes { get; set; }
   public float Width { get; set; }
   public float Height { get; set; }
}