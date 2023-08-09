using ActionableEmailsTest;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Diagnostics;
using System.Net;

namespace ActionableEmailsTest.Controllers;



[ApiController]
[Route("[controller]")]
public class MessageEndpointsController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    static string test = "";

    private readonly ILogger<WeatherForecastController> _logger;

    public MessageEndpointsController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetMessage")]
    public IEnumerable<Message> GetMessage()
    {
        var data = test;
        return new[] { new Message() { Data = data } };
    }


    [HttpPost(Name = "PostMessage")]
    public Message PostMessage(Message model)
    {
        test = model.Data;



        return model;
    }
}

