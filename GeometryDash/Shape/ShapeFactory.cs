using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics;

namespace CringeCraft.GeometryDash.Shape;

public class ShapeMetadata {
    public string Name { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
}

public static class ShapeFabric {
    private class ImportInfo {
        [ImportMany]
        public IEnumerable<Lazy<IShape, ShapeMetadata>> AvailableShapes { get; set; } = [];
    }

    private static readonly ImportInfo info = new();

    static ShapeFabric() {
        try {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var conf = new ContainerConfiguration().WithAssemblies(assemblies);
            using var cont = conf.CreateContainer();
            cont.SatisfyImports(info);
        } catch (Exception ex) {
            Debug.WriteLine($"Error loading assemblies: {ex.Message}");
        }
    }

    public static IEnumerable<string> AvailableShapes => info.AvailableShapes.Select(f => f.Metadata.Name);
    public static IEnumerable<string> AvailableShapesIcon => info.AvailableShapes.Select(f => f.Metadata.Icon);

    public static IShape? CreateShape(string shapeName, params object[] args) {
        var shapeInfo = info.AvailableShapes.FirstOrDefault(f => f.Metadata.Name == shapeName);
        if (shapeInfo == null) return null;

        try {
            return (IShape?)Activator.CreateInstance(shapeInfo.Value.GetType(), args);
        } catch (Exception ex) {
            Debug.WriteLine($"Error creating shape {shapeName}: {ex.Message}");
            return null;
        }
    }
}
