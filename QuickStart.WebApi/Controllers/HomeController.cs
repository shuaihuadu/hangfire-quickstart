using Microsoft.AspNetCore.Mvc;

namespace QuickStart.WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public string Ping()
    {
        return "Pong";
    }
}