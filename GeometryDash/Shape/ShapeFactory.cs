using System.Composition;
using System.Composition.Hosting;

namespace CringeCraft.GeometryDash.Shape;

public interface ShapeMetadata {
    string Name { get; }
}

public static class ShapeFabric {
    class ImportInfo {
        [ImportMany]
        public IEnumerable<Lazy<IShape, ShapeMetadata>> AvailableFigures { get; set; } = [];
    }
    static readonly ImportInfo info;
    static ShapeFabric() {
        //Возможен CRINGE, если другие классы содержатся в других сборках
        var assemblies = new[] { typeof(Line).Assembly };
        var conf = new ContainerConfiguration();
        try {
            conf = conf.WithAssemblies(assemblies);
        } catch (Exception) {
            // ignored
        }

        var cont = conf.CreateContainer();
        info = new ImportInfo();
        cont.SatisfyImports(info);
    }

    public static IEnumerable<string> AvailableFigures => info.AvailableFigures.Select(f => f.Metadata.Name);
    //Выбирает конструктор без параметров
    public static IShape CreateFigure(string FigureName) {
        return info.AvailableFigures.First(f => f.Metadata.Name == FigureName).Value;
    }
}
