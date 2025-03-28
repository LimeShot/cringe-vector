using Microsoft.Win32;
using System.IO;
using CringeCraft.IO;
using CringeCraft.GeometryDash;
using CringeCraft.Client.Model.Canvas;

public static class FileService {
    public static (string? filePath, string? errorMessage) OpenFile(MyCanvas canvas) {
        OpenFileDialog openFileDialog = new() {
            Title = "Открыть файл",
            Filter = "CRNG файлы (*.crng)|*.crng|Все файлы (*.*)|*.*",
            DefaultExt = ".crng",
            FileName = "document.crng"
        };

        if (openFileDialog.ShowDialog() == true) {
            string filePath = openFileDialog.FileName;
            try {
                IImporter importer = filePath.EndsWith(".crng", StringComparison.OrdinalIgnoreCase)
                    ? new ImportFromCRNG()
                    : new ImportFromCRNG();
                canvas.Shapes.Clear();
                canvas.SelectedShapes.Clear();
                canvas.GetGeneralBB = null;
                var (shapes, width, height) = importer.Import(filePath);
                if (shapes != null)
                    canvas.Shapes = shapes;
                else
                    throw new Exception();
                canvas.Width = width;
                canvas.Height = height;

                if (shapes.Count > 0) {
                    canvas.IsShapesVisible = System.Windows.Visibility.Visible;
                }

                return (filePath, null);
            } catch (Exception) {
                return (null, $"Ошибка при открытии файла.");
            }
        }
        return (null, null); // Пользователь отменил выбор
    }

    public static (string? filePath, string? errorMessage) SaveFile(ICanvas canvas) {
        SaveFileDialog saveFileDialog = new() {
            Title = "Сохранить файл",
            Filter = "CRNG файлы (*.crng)|*.crng|SVG файлы (*.svg)|*.svg|PNG файлы (*.png)|*.png|Все файлы (*.*)|*.*",
            DefaultExt = ".crng",
            FileName = "document.crng"
        };

        if (saveFileDialog.ShowDialog() == true) {
            string filePath = saveFileDialog.FileName;
            try {
                IExporter exporter = filePath.EndsWith(".crng", StringComparison.OrdinalIgnoreCase)
                    ? new ExportToCRNG()
                    : filePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
                        ? new ExportToSVG()
                        : filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                            ? new ExportToPNG()
                            : new ExportToCRNG();

                exporter.Export(filePath, canvas);
                return (filePath, null);
            } catch (Exception ex) {
                return (null, $"Ошибка при сохранении файла: {ex.Message}");
            }
        }
        return (null, null);
    }
}
