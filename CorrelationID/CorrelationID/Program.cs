
var builder = WebApplication.CreateBuilder(args);

// Required to create HttpClient.
builder.Services.AddHttpClient();


var app = builder.Build();

app.MapGet("/api/orders/{id:int}", async (int id,HttpContext context,IHttpClientFactory httpClientFactory) =>
{
    // Read the incoming correlation ID.
    string correlationId = context.Request.Headers["X-Correlation-ID"].ToString();

    // Generate an ID if the client did not provide one.
    if (string.IsNullOrWhiteSpace(correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
    }

    // Return the ID to the original client.
    context.Response.Headers["X-Correlation-ID"] = correlationId;

    app.Logger.LogInformation(
        "Order API | ID: {Id} | Getting order {OrderId}",
        correlationId, id);

    var client = httpClientFactory.CreateClient();

    // Prepare the request to API 2.
    using var request = new HttpRequestMessage(HttpMethod.Get,$"http://localhost:5002/api/payments/{id}");

    // IMPORTANT: Forward the same ID to API 2.
    request.Headers.Add("X-Correlation-ID", correlationId);

    using var response = await client.SendAsync(request);

    response.EnsureSuccessStatusCode();

    string paymentStatus =
        await response.Content.ReadAsStringAsync();

    app.Logger.LogInformation(
        "Order API | ID: {Id} | Payment response: {Status}",
        correlationId, paymentStatus);

    return Results.Ok(new
    {
        OrderId = id,
        Product = "Laptop",
        PaymentStatus = paymentStatus,
        CorrelationId = correlationId
    });
});

app.Run("http://localhost:5001");