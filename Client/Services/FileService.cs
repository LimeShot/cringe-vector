using Microsoft.Win32;
using System.IO;
using CringeCraft.IO;
using CringeCraft.GeometryDash;

public static class FileService {
    public static string? OpenFile() {
        OpenFileDialog openFileDialog = new OpenFileDialog {
            Title = "Открыть файл",
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
        };

        return openFileDialog.ShowDialog() == true ? File.ReadAllText(openFileDialog.FileName) : null;
    }

    public static string? SaveFile(ICanvas canvas) {
        SaveFileDialog saveFileDialog = new SaveFileDialog {
            Title = "Сохранить файл",
            Filter = "CRNG файлы (*.crng)|*.crng|SVG файлы (*.svg)|*.svg|Все файлы (*.*)|*.*",
            DefaultExt = ".crng",
            FileName = "document.crng"
        };

        if (saveFileDialog.ShowDialog() == true) {
            string filePath = saveFileDialog.FileName;
            IExporter exporter;

            if (filePath.EndsWith(".crng", StringComparison.OrdinalIgnoreCase)) {
                exporter = new ExportToCRNG();
            } else if (filePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) {
                exporter = new ExportToSVG();
            } else {
                exporter = new ExportToCRNG();
            }

            //Я кринжанул(Тимофей)
            //exporter.Export(filePath, canvas);
            return filePath;
        }

        return null;
    }
}
