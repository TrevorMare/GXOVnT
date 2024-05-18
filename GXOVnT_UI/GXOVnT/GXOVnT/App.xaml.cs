using GXOVnT.Services;
using GXOVnT.Services.Interfaces;

namespace GXOVnT;

public partial class App : Application
{

    //https://github.com/dotnet-bluetooth-le/dotnet-bluetooth-le/tree/master/Source/BLE.Client
    
    public App(IServiceProvider provider)
    {
        InitializeComponent();
        Class1.Initialize(provider);
        MainPage = new MainPage();
    }
}