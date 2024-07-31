using MatoEditor.Views;

namespace MatoEditor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public Category Category { get; set; }

    public MainWindowViewModel()
    {
        Category = new Category();
    }
}