// Call this via HTTP POST with a URL like:
// http://localhost:9998/findExternalDeliveryDriver
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.MapPost("/findExternalDeliveryDriver", async (HttpContext context) =>
{
    logger.LogInformation("Checking UberEats...");
    await Task.Delay(500);

    logger.LogInformation("Checking Grubhub...");
    await Task.Delay(500);

    logger.LogInformation("Checking DoorDash... Responded.");
    await Task.Delay(500);

    return Results.Json(new { service = "DoorDash" });
});

app.Run("http://localhost:9998");