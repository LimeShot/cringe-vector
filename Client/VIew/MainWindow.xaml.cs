namespace CringeCraft.Client.View;

using System.Windows;
using OpenTK.Wpf;
using CringeCraft.Client.ViewModel;

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