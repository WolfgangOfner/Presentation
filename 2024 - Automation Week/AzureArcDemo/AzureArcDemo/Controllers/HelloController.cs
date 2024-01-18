using Microsoft.AspNetCore.Mvc;

namespace AzureArcDemo.Controllers;

[Route("[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    /// <summary>
    /// Get greeting message
    /// </summary>
    /// <returns>Returns the string greeting message</returns>
    [HttpGet]
    public string Hello()
    {
        return "Hello Azure Arc!";
    }
}