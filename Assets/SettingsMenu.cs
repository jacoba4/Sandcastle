using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider slider;
    public Dropdown dropdown;
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("MasterVol", 0.75f);
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetVolume ()
    {
        float sliderValue = slider.value;
        audioMixer.SetFloat("MasterVol", Mathf.Log10 (sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVol", sliderValue);
    }
    public void SetGraphics()
    {
        int dropdownvalue = dropdown.value;
        QualitySettings.SetQualityLevel(dropdownvalue);
    }
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}