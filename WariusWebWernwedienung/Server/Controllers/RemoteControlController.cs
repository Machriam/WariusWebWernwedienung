using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WariusWebWernwedienung.Shared;

namespace WariusWebWernwedienung.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemoteControlController : ControllerBase
{
    private const string ScriptFolderName = "ScriptFolder";
    private readonly IConfiguration _configuration;

    public RemoteControlController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IEnumerable<string> GetExecutableScripts()
    {
        var path = _configuration.GetConnectionString(ScriptFolderName)
            ?? throw new Exception($"Connection {ScriptFolderName} must be defined in appsettings.json");
        return Directory.GetFiles(path).Select(f => Path.GetFileName(f)).Where(p => p.EndsWith(".exe"));
    }

    [HttpPost]
    public bool Post([FromBody] RemoteControlParameter parameter)
    {
        var filePath = _configuration.GetConnectionString(ScriptFolderName);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(filePath + "\\" + Path.GetFileName(parameter.FileName), parameter.Parameter)
        };
        return process.Start();
    }
}
