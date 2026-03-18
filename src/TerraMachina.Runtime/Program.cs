using Scalar.AspNetCore;
using TerraMachina.Runtime.Hubs;
using TerraMachina.Runtime.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IEngineService, EngineService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevClient", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("DevClient");
app.MapControllers();
app.MapHub<EngineHub>("/hubs/engine");

app.Run();
