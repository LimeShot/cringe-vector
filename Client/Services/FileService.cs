using Microsoft.Win32;

using System.IO;

public static class FileService {
    public static string? OpenFile() {
        OpenFileDialog openFileDialog = new OpenFileDialog {
            Title = "Открыть файл",
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
        };

        return openFileDialog.ShowDialog() == true ? File.ReadAllText(openFileDialog.FileName) : null;
    }

    public static string? SaveFile(string content) {
        SaveFileDialog saveFileDialog = new SaveFileDialog {
            Title = "Сохранить файл",
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = "document.txt"
        };

        if (saveFileDialog.ShowDialog() == true) {
            File.WriteAllText(saveFileDialog.FileName, content);
            return saveFileDialog.FileName;
        }

        return null;
    }
}