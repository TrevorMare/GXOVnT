
namespace GXOVnT;

public partial class App : Application
{
    
    public App(IServiceProvider services)
    {
        InitializeComponent();
        MainPage = new MainPage();
    }
}