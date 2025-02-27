using System.ComponentModel;
using System.Collections.ObjectModel;
using CringeCraft.GeometryDash.Shape;

namespace CringeCraft.GeometryDash.Canvas;

public interface ICanvas : INotifyPropertyChanged {
   public ObservableCollection<IShape> Shapes { get; set; }
   public float LengthCanvas { get; set; }
   public float WeightCanvas { get; set; }

}