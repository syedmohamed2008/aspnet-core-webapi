using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    // Return 429 when the request limit is exceeded.
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("FixedPolicy", policy =>
    {
        policy.PermitLimit = 3;                    // Allow 3 requests
        policy.Window = TimeSpan.FromSeconds(30); // Every 30 seconds
        policy.QueueLimit = 0;                     // Reject extra requests
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
