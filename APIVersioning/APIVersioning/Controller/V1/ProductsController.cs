using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiVersioning.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrderController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Id = 1,
            Name = "Laptop"
        });
    }
}