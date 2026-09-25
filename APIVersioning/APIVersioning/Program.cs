using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddApiVersioning(options =>
    {
        // Configure V1 as the default API version.
        options.DefaultApiVersion = new ApiVersion(1, 0);

        // Clients must explicitly specify a version.
        //options.AssumeDefaultVersionWhenUnspecified = false;

        // Use V1 when the request does not specify a version.
        options.AssumeDefaultVersionWhenUnspecified = true;

        // Include supported/deprecated version information in headers.
        options.ReportApiVersions = true;

        // Read the version from URLs such as /api/v1/Products.
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
