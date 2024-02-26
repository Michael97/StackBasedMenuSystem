using UnityEngine.UI;
using UnityEngine;
using TMPro;
using StackBasedMenuSystem;

public class OptionsMenu : SimpleMenu<OptionsMenu>
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Dropdown graphicsQualityDropdown;

    public override void BindButtonActions()
    {
        base.BindButtonActions();

        // Load current settings
        musicVolumeSlider.value = SaveLoadPlayerSettings.LoadMusicVolume();
        sfxVolumeSlider.value = SaveLoadPlayerSettings.LoadSFXVolume();
        graphicsQualityDropdown.value = (int)SaveLoadPlayerSettings.LoadGraphicsQuality();

        FindButtonAndAddListener("SaveButton", OnSaveButton);
    }

    public void OnSaveButton()
    {
        // Save player preferences here
        SaveLoadPlayerSettings.SaveMusicVolume(musicVolumeSlider.value);
        SaveLoadPlayerSettings.SaveSFXVolume(sfxVolumeSlider.value);
        SaveLoadPlayerSettings.SaveGraphicsQuality(graphicsQualityDropdown.value);

        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChange(float volume)
    {
        // Handle music volume change here
        Debug.Log($"Music volume changed to {volume}");
    }

    public void OnSFXVolumeChange(float volume)
    {
        // Handle SFX volume change here
        Debug.Log($"SFX volume changed to {volume}");
    }

    public void OnGraphicsQualityChange(int index)
    {
        // Handle graphics quality change here
        Debug.Log($"Graphics quality changed to {index}");
    }

    public override void OnEnable()
    {
        base.OnEnable();

        // Load player preferences here
        musicVolumeSlider.value = SaveLoadPlayerSettings.LoadMusicVolume();
        sfxVolumeSlider.value = SaveLoadPlayerSettings.LoadSFXVolume();
        graphicsQualityDropdown.value = (int)SaveLoadPlayerSettings.LoadGraphicsQuality();
    }

    public override void OnBackPressed()
    {
        base.OnBackPressed();

        OnSaveButton();
    }


}
