using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiVersioning.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrderController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetV1()
    {
        return Ok(new
        {
            Id = 1,
            Name = "Laptop"
        });
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetV2()
    {
        return Ok(new
        {
            Id = 1,
            Name = "Laptop",
            Price = 50000
        });
    }
}