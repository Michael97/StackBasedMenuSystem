using UnityEditor;
using UnityEngine;

public static class SaveLoadPlayerSettings
{
    private static readonly string MusicVolumeKey = "MusicVolume";
    private static readonly string SFXVolumeKey = "SFXVolume";
    private static readonly string GraphicsQualityKey = "GraphicsQuality";

    private static readonly float defaultMusicVolume = 0.5f;
    private static readonly float defaultSFXVolume = 0.5f;
    private static readonly float defaultGraphicsQuality = 2;

    private static ISettingsService settingsService = new PlayerPrefSettingsService();

    public static void SaveMusicVolume(float volume)
    {
        settingsService.Save(MusicVolumeKey, volume);
    }

    public static float LoadMusicVolume()
    {
        return settingsService.Load(MusicVolumeKey, defaultMusicVolume);
    }

    public static void SaveSFXVolume(float volume)
    {
        settingsService.Save(SFXVolumeKey, volume);
    }

    public static float LoadSFXVolume()
    {
        return settingsService.Load(SFXVolumeKey, defaultSFXVolume);
    }

    public static void SaveGraphicsQuality(float quality)
    {
        settingsService.Save(GraphicsQualityKey, quality);
    }

    public static float LoadGraphicsQuality()
    {
        return settingsService.Load(GraphicsQualityKey, defaultGraphicsQuality);
    }
}
