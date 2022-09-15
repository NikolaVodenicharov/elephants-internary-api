using WebAPI.Common;
using WebAPI.Common.ErrorHandling;
using WebAPI.Common.Extensions;
using WebAPI.Common.Extensions.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.GetSection(ApplicationSettings.SectionName).Get<ApplicationSettings>();
var azureSettings = builder.Configuration.GetSection(AzureSettings.SectionName).Get<AzureSettings>();

builder.Services.CustomizeIdentity(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.CustomizeSwagger(appSettings);

builder.Services.ConfigureAndMigrateDatabase(builder.Configuration);

builder.Services.CustomizeCorsPolicy(appSettings);
builder.Services.CustomizeDependencies();
builder.Services.CustomizeRouting();
builder.SetupLogger();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( config => 
    {
        config.OAuthUsePkce();
        config.OAuthClientId(azureSettings.ClientId);
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
