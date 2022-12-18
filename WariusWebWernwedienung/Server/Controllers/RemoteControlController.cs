using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WariusWebWernwedienung.Shared;

namespace WariusWebWernwedienung.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemoteControlController : ControllerBase
{
    private const string ScriptFolderName = "ScriptFolder";
    private readonly IConfiguration _configuration;
    private readonly string _scriptFolder;
    private const string ChromeDriver = "chromedriver.exe";

    public RemoteControlController(IConfiguration configuration)
    {
        _configuration = configuration;
        _scriptFolder = _configuration.GetConnectionString(ScriptFolderName)
            ?? throw new Exception($"Connection {ScriptFolderName} must be defined in appsettings.json");
    }

    [HttpGet]
    public IEnumerable<string> GetExecutableScripts()
    {
        return Directory.GetFiles(_scriptFolder)
            .Select(f => Path.GetFileName(f))
            .Where(p => p != ChromeDriver && (p.EndsWith(".bat") || p.EndsWith(".exe")));
    }

    [HttpGet("links")]
    public IEnumerable<HtmlLink> GetLinks()
    {
        var driver = new ChromeDriver(_scriptFolder + "/" + ChromeDriver,
            new ChromeOptions()
            {
                DebuggerAddress = "localhost:9222",
            });
        driver.Navigate().GoToUrl("https://bs.to");
        return new List<HtmlLink>();
        //driver.FindElements(By.)
    }

    [HttpPost]
    public bool Post([FromBody] RemoteControlParameter parameter)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(_scriptFolder + "\\" + Path.GetFileName(parameter.FileName),
                                             "\"" + parameter.Parameter + "\"")
        };
        return process.Start();
    }
}
