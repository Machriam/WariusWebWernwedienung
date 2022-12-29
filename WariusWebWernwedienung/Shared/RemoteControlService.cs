namespace WariusWebWernwedienung.Shared;

public interface IRemoteControlService
{
    Task<bool> SendRemoteControl(RemoteControlParameter parameter);

    Task<IEnumerable<string>?> GetExecutableScripts();
    Task Navigate(HtmlLink link);
    Task<IEnumerable<HtmlLink>?> GetLinks();
}

public class RemoteControlService : IRemoteControlService
{
    private readonly IRestInterop _restInterop;
    private const string BaseRoute = "api/remotecontrol";

    public RemoteControlService(IRestInteropFactory restInteropFactory)
    {
        _restInterop = restInteropFactory.CreateRestInterop(BaseRoute);
    }

    public async Task<IEnumerable<string>?> GetExecutableScripts()
    {
        return await _restInterop.GetAsync<IEnumerable<string>>("");
    }

    public async Task<bool> SendRemoteControl(RemoteControlParameter parameter)
    {
        return await _restInterop.PostAsync<bool, RemoteControlParameter>("", parameter);
    }

    public async Task Navigate(HtmlLink link)
    {
        await _restInterop.PostAsync("navigate", link);
    }

    public async Task<IEnumerable<HtmlLink>?> GetLinks()
    {
        return await _restInterop.GetAsync<IEnumerable<HtmlLink>>("links");
    }
}
