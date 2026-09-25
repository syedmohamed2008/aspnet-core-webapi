var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/payments/{orderId:int}", (int orderId, HttpContext context) =>
{
    // Read the correlation ID forwarded by API 1.
    string correlationId = context.Request.Headers["X-Correlation-ID"].ToString();

    // Also support requests made directly to API 2.
    if (string.IsNullOrWhiteSpace(correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
    }

    context.Response.Headers["X-Correlation-ID"] = correlationId;

    app.Logger.LogInformation(
        "Payment API | ID: {Id} | Checking payment for order {OrderId}",
        correlationId, orderId);

    return Results.Text("Paid");
});

app.Run("http://localhost:5002");