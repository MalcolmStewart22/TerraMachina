using Scalar.AspNetCore;
using TerraMachina.Runtime.Hubs;
using TerraMachina.Runtime.JSON;
using TerraMachina.Runtime.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = null; // no limit
    options.StreamBufferCapacity = 20;
});
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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new Vector3JsonConverter());
    });
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.PayloadSerializerOptions.Converters.Add(new Vector3JsonConverter());
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
