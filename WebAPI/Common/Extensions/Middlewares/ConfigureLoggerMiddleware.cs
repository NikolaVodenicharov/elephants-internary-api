namespace WebAPI.Common.Extensions.Middlewares;
public static class ConfigureLoggerMiddleware
{
    public static void SetupLogger(this WebApplicationBuilder builder)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var logFolder = "Logs";
        var logFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_log.txt";
        var logFilePath = Path.Combine(currentDirectory, logFolder, logFileName);

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.AddFile(logFilePath);
        });
    }
}
