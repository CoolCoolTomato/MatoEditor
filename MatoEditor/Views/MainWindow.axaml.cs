using Avalonia.Controls;
using MatoEditor.ViewModels;

namespace MatoEditor.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}