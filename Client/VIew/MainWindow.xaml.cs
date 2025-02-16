using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;

namespace CringeCraft.Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        var settings = new GLWpfControlSettings {
            MajorVersion = 3,
            MinorVersion = 0
        };
        OpenTkControl.Start(settings);
    }

    private void OnRender(TimeSpan delta) {
        Random random = new Random();
        GL.ClearColor(new OpenTK.Mathematics.Color4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1f));
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
}