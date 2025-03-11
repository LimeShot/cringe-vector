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
        CringeCanvas cringe = new();
/*
        Rectangle rig = new();
        Line lin = new();    
        cringe.AddShape(lin);    
        cringe.AddShape(rig);   
        ExportToCRNG exp = new();
        ImportFromCRNG imp = new();
        exp.Export(@"C:\Users\1\Desktop\pepe.crng",cringe);
        CringeCanvas necringe = imp.Import(@"C:\Users\1\Desktop\pepe.crng");
        ExportToCRNG expo = new();
        expo.Export(@"C:\Users\1\Desktop\pepepepe.crng",cringe);
*/
        MainViewModel mViewModel = new(this);
        DataContext = mViewModel;

        MainWindowElement.Closing += mViewModel.Closing;
        OpenTkControl.SizeChanged += mViewModel.Resize;
        OpenTkControl.Ready += mViewModel.InitializeOpenGL;
        OpenTkControl.Render += mViewModel.Render;

        OpenTkControl.Start(new());
    }
}