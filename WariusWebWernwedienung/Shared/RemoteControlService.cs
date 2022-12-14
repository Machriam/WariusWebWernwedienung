namespace WariusWebWernwedienung.Shared;

public interface IRemoteControlService
{
    Task<bool> SendRemoteControl(RemoteControlParameter parameter);

    Task<IEnumerable<string>?> GetExecutableScripts();
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
}
