using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{

    [Header("Gráficos")]
    [SerializeField] private TMP_Dropdown qualityDropDown; 
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private TMP_Dropdown fpsDropDown;
    [Header("Full Screen")]
    [SerializeField] private Toggle fullScreenToggle;
    [Header("Sound")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambientSlider;
    private DataSettings dataSettings;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSettings();   
        SetUIElements();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SettingsData") == true)
        {
            string data = PlayerPrefs.GetString("SettingsData");
            dataSettings = JsonUtility.FromJson<DataSettings>(data);
        }
        else
        {
            dataSettings = new DataSettings();
            SetDefauldDataValues();
        }

            dataSettings = new DataSettings();
    }

    void SetDefauldDataValues()
    {
        dataSettings.musicVolume = 1f;
        dataSettings.ambientVolume = 1f;
        dataSettings.fullScreen = true;
        dataSettings.quality = 1;
        dataSettings.fps = 1;

        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                dataSettings.resolution = 1;
                break;
            }
        }
    }

    private void SaveSettings()
    {
        string data = JsonUtility.ToJson(dataSettings);
        PlayerPrefs.SetString("SettingsData", data);
    }

    private void SetUIElements()
    {
        //Sliders de volumen
        musicSlider.value = dataSettings.musicVolume;
        ambientSlider.value = dataSettings.ambientVolume;
        //Toggle Full Screen
        fullScreenToggle.isOn = dataSettings.fullScreen;
        //Dropdown FPS
        fpsDropDown.value = dataSettings.fps;
        //Dropdown resolution
        Resolution[] resolutionOptions = Screen.resolutions;
        for(int i = 0; i < resolutionOptions.Length; i++)
        {
            string option = resolutionOptions[i].width.ToString() +"x"+ resolutionOptions[i].height.ToString();
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(option);
            resolutionDropDown.options.Add(optionData);
        }
        resolutionDropDown.value = dataSettings.resolution;
        //Dropdown Quality
        qualityDropDown.ClearOptions();
        List<TMP_Dropdown.OptionData> optionsQuality = new List<TMP_Dropdown.OptionData>();
        for (int i = 0;i < QualitySettings.names.Length;i++)
        {
            optionsQuality.Add(new TMP_Dropdown.OptionData(QualitySettings.names[i]));
        }
        qualityDropDown.AddOptions(optionsQuality);
        qualityDropDown.value = dataSettings.quality;
    }

    public void ApplyButton()
    {
        //Aplicar sonido
        dataSettings.musicVolume = musicSlider.value;
        AudioManager.instance.SetMusicVolume(dataSettings.musicVolume);
        dataSettings.ambientVolume = ambientSlider.value;
        AudioManager.instance.SetSFXVolume(dataSettings.ambientVolume);

        //Aplicar Full Screen
        dataSettings.fullScreen = fullScreenToggle.isOn;
        Screen.fullScreen = dataSettings.fullScreen;

        //Aplicar FPS
        dataSettings.fps = fpsDropDown.value;
        switch (dataSettings.fps)
        {
            case 0:
                Application.targetFrameRate = -1;
                break;

            case 1:
                Application.targetFrameRate = 120;
                break;

            case 2:
                Application.targetFrameRate = 60;
                break;

            case 3:
                Application.targetFrameRate = 30;
                break;

            case 4:
                Application.targetFrameRate = 3;
                break;
        }

        //Aplicar Quality
        dataSettings.quality = qualityDropDown.value;
        QualitySettings.SetQualityLevel(dataSettings.quality);

        //Aplicar Resolucion
        dataSettings.resolution = resolutionDropDown.value;
        Resolution resolution = Screen.resolutions[dataSettings.resolution];
        Screen.SetResolution(resolution.width, resolution.height, dataSettings.fullScreen);

        SaveSettings();
    }

    public void BackButton()
    {

    }
}

public class DataSettings
{
    public float musicVolume;
    public float ambientVolume;
    public bool fullScreen;
    public int fps;
    public int quality;
    public int resolution;
}
