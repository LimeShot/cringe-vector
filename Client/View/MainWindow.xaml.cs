namespace CringeCraft.Client.View;

using System.Windows;
using OpenTK.Wpf;
using CringeCraft.Client.ViewModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        MainViewModel mViewModel = new(this);
        DataContext = mViewModel;

        MainWindowElement.Closing += mViewModel.Closing;
        OpenTkControl.SizeChanged += mViewModel.Resize;
        OpenTkControl.Ready += mViewModel.InitializeOpenGL;
        OpenTkControl.Render += mViewModel.Render;

        OutlineColorPicker.Opened += (s, e) => PopupStateManager.NotifyPopUpOpened();
        FillColorPicker.Opened += (s, e) => PopupStateManager.NotifyPopUpOpened();

        // Ну это полный кринж
        OutlineColorPicker.Closed += async (s, e) => {
            await Task.Delay(100); // Задержка в 100 миллисекунд
            PopupStateManager.NotifyPopUpClosed();
        };
        FillColorPicker.Closed += async (s, e) => {
            await Task.Delay(100); // Задержка в 100 миллисекунд
            PopupStateManager.NotifyPopUpClosed();
        };

        OpenTkControl.Start(new());
    }
}