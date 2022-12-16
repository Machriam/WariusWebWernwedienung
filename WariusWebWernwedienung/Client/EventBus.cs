namespace WariusWebWernwedienung.Client;

public class EventBus
{
    public event Action? ButtonClicked;
    public void OnButtonClicked() => ButtonClicked?.Invoke();
}
