using WebAPI.Common.ErrorHandling;
using WebAPI.Common.Extensions;
using WebAPI.Common.Extensions.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureAndMigrateDatabase(builder.Configuration);

builder.Services.CustomizeCorsPolicy(builder.Configuration);
builder.Services.CustomizeDependencies();
builder.Services.CustomizeRouting();
builder.SetupLogger();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
