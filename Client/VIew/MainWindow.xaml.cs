namespace CringeCraft.Client.View;
using CringeCraft.Client.Model.Canvas;
using System.Windows;
using OpenTK.Wpf;
using CringeCraft.Client.ViewModel;
using System.Windows.Input;
using CringeCraft.GeometryDash.Shape;
using CringeCraft.GeometryDash;

using CringeCraft.IO;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        MainViewModel mViewModel = new(this);
        DataContext = mViewModel;

        MainWindowElement.Closing += mViewModel.Closing;
        OpenTkControl.SizeChanged += mViewModel.Resize;
        OpenTkControl.Ready += mViewModel.InitializeOpenGL;
        OpenTkControl.Render += mViewModel.Render;

        OpenTkControl.Start(new());
    }
}