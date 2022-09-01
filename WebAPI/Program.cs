using Infrastructure;
using Microsoft.EntityFrameworkCore;
using WebAPI.Common.ErrorHandling;
using WebAPI.Common.Extensions.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<InternaryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InternaryDatabaseConnection")));

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

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
