using Microsoft.Extensions.Logging;
using MauiBlazorCrudOfflineFirst.Shared.Services;
using MauiBlazorCrudOfflineFirst.Services;
using MauiBlazorCrudOfflineFirst.Shared.Core.Services;

namespace MauiBlazorCrudOfflineFirst;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add device-specific services used by the MauiBlazorCrudOfflineFirst.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();
        
        // Path to SQLite database
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "LocalDatabase.db");
        
        // Change this:
builder.Services.AddSingleton(new ProductService(dbPath));

        builder.Services.AddScoped<SyncService>();
        builder.Services.AddScoped(sp => new HttpClient());

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
