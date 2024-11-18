using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace TNAPP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Cambiar el color primario
            builder.Services.AddSingleton<App>(sp => new App { Resources = new ResourceDictionary { { "Primary", Color.FromArgb("#0078D7") }, // Cambia el código hexadecimal por el color que prefieras
 } });
            return builder.Build();
        }
    }
}