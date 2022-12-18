namespace WariusWebWernwedienung.Client;

public class SectionInformation
{
    public string Name { get; set; } = "";
    public List<ButtonInformation> ButtonInformation { get; set; } = new();
}
public class ButtonInformation
{
    public string Filename { get; set; } = "";
    public string Parameter { get; set; } = "";
    public string Label { get; set; } = "";
    public bool DynamicParameter { get; set; }
}
