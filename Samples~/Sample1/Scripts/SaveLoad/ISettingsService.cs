public interface ISettingsService
{
    void Save(string key, float value);
    float Load(string key, float defaultValue);
}
