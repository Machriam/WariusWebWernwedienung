namespace WariusWebWernwedienung.Client;

public interface IDataStorage
{
    Task DeleteSection(SectionInformation sectionInformation);
    Task UpdateSection(SectionInformation sectionInformation);
}

public class DataStorage : IDataStorage
{
    public async Task UpdateSection(SectionInformation sectionInformation)
    {
    }
    public async Task DeleteSection(SectionInformation sectionInformation)
    {
    }
}
