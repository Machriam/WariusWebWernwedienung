using System.Text.Json;

namespace WariusWebWernwedienung.Client;

public interface IDataStorage
{
    Task DeleteSection(SectionInformation sectionInformation);
    Task<IEnumerable<SectionInformation>> GetSectionInformation();
    Task UpdateSection(SectionInformation sectionInformation);
}

public class DataStorage : IDataStorage
{
    private readonly ILocalStorageInterop _localStorageInterop;
    private const string LocalStorageSections = nameof(LocalStorageSections);

    public DataStorage(ILocalStorageInterop localStorageInterop)
    {
        _localStorageInterop = localStorageInterop;
    }
    public async Task<IEnumerable<SectionInformation>> GetSectionInformation()
    {
        var information = await _localStorageInterop.GetItem(LocalStorageSections);
        if (string.IsNullOrEmpty(information)) return new List<SectionInformation>();
        return JsonSerializer.Deserialize<IEnumerable<SectionInformation>>(information) ?? new List<SectionInformation>();
    }

    public async Task UpdateSection(SectionInformation sectionInformation)
    {
        var information = await GetSectionInformation();
        var existingSection = information.FirstOrDefault(i => i.Name == sectionInformation.Name);
        if (existingSection == null) information = information.Append(sectionInformation);
        else existingSection.ButtonInformation = sectionInformation.ButtonInformation;
        await _localStorageInterop.SetItem(LocalStorageSections, JsonSerializer.Serialize(information));
    }
    public async Task DeleteSection(SectionInformation sectionInformation)
    {
        var information = await GetSectionInformation();
        var newSections = information.Where(i => i.Name != sectionInformation.Name);
        await _localStorageInterop.SetItem(LocalStorageSections, JsonSerializer.Serialize(newSections));
    }
}
