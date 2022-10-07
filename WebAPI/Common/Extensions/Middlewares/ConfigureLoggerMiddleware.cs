namespace WebAPI.Common.Extensions.Middlewares;
public static class ConfigureLoggerMiddleware
{
    private const long maxFileSize = 10 * 1024 * 1024; //10 MB

    internal static void SetupLogger(this WebApplicationBuilder builder, bool isProduction)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var logFolder = "Logs";
        var logFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_log.txt";
        var logFilePath = Path.Combine(currentDirectory, logFolder, logFileName);

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();

            if (!isProduction)
            {
                logging.AddConsole();
                logging.AddDebug();
            }

            logging.AddFile(logFilePath, fileSizeLimitBytes: maxFileSize);
        });
    }
}
