namespace GXOVnT.Shared;

public static class AppService
{

    public static IServiceProvider ServiceProvider { get; private set; } = default!;

    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
    
    
}