namespace WariusWebWernwedienung.Client;

public class EventBus
{
    public event Action? ButtonClicked;
    public event Func<SectionInformation, Task>? SectionSelected;
    public void OnButtonClicked() => ButtonClicked?.Invoke();
    public Task OnSectionSelected(SectionInformation section) => SectionSelected?.Invoke(section) ?? Task.CompletedTask;
}
