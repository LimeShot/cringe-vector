﻿using System.Text;
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
using CringeCraft.Client.ViewModel;
using CringeCraft.Client.Render;

namespace CringeCraft.Client.View;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        MainViewModel WViewModel = new MainViewModel(this);
        DataContext = WViewModel;

        MainWindowElement.Closing += WViewModel.Closing;
        OpenTkControl.SizeChanged += WViewModel.Resize;
        OpenTkControl.Ready += WViewModel.InitializeOpenGL;
        OpenTkControl.Render += WViewModel.Render;

        var settings = new GLWpfControlSettings {
            MajorVersion = 3,
            MinorVersion = 0
        };
        OpenTkControl.Start(settings);
    }
}