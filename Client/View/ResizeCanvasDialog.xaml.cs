using System;
using System.Windows;

namespace VectorPaint
{
    public partial class ResizeCanvasDialog : Window
    {
        public int CanvasWidth { get; private set; }
        public int CanvasHeight { get; private set; }

        public ResizeCanvasDialog(int currentWidth, int currentHeight)
        {
            InitializeComponent();
            WidthTextBox.Text = currentWidth.ToString();
            HeightTextBox.Text = currentHeight.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WidthTextBox.Text, out int newWidth) &&
                int.TryParse(HeightTextBox.Text, out int newHeight) &&
                newWidth > 0 && newHeight > 0)
            {
                CanvasWidth = newWidth;
                CanvasHeight = newHeight;
                
                this.Tag = true; // Флаг успешного ввода
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите корректные размеры!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = false; // Флаг отмены
            this.Close();
        }
    }
}
