using UnityEditor;
using UnityEngine;

public class PlayerPrefSettingsService : ISettingsService
{
    public void Save(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public float Load(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
}
