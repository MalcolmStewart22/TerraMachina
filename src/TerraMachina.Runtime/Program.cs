using Scalar.AspNetCore;
using TerraMachina.Runtime.Hubs;
using TerraMachina.Runtime.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IEngineService, EngineService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<EngineHub>("/hubs/engine");

app.Run();
