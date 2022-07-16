using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class SettingsHandler : MonoBehaviour
{
    public PlayerInput inputs;

    public AudioMixer mixer;
    public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown qualitySelector;
    public TMP_Dropdown resolutionSelector;
    public Toggle fullScreenToggle;
    public Toggle VSyncToggle;
    public PlayerController player;
    public Slider sensitivitySlider, fovSlider, volumeSlider;

    private Canvas settingsMenu;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        sensitivitySlider.value = GlobalSettings.getSensitivity();
        fovSlider.value = GlobalSettings.getFov();
        volumeSlider.value = GlobalSettings.getVolume();
        player.setSensitivity(sensitivitySlider.value);
        player.setFov(fovSlider.value);
        resolutions = Screen.resolutions;
        resolutionSelector.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }

        }



        resolutionSelector.AddOptions(resolutionOptions);
        resolutionSelector.value = currentResolutionIndex;
        resolutionSelector.RefreshShownValue();
        //Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, fullScreenToggle.isOn);

        fullScreenToggle.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
            VSyncToggle.isOn = false;
        else
            VSyncToggle.isOn = true;

        qualitySelector.value = QualitySettings.GetQualityLevel();
        qualitySelector.RefreshShownValue();

        inputs = new PlayerInput();
        inputs.Menu.Menu.performed += pause;
        inputs.Menu.Menu.Enable();

        settingsMenu = GetComponent<Canvas>();
        settingsMenu.enabled = false;
    }

    void OnDestroy()
    {
        inputs.Menu.Menu.Disable();
    }

    void pause(InputAction.CallbackContext obj)
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        settingsMenu.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.enterMenu();
    }

    public void resume()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        settingsMenu.enabled = false;
        player.exitMenu();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void changeVolume(float volume)
    {
        mixer.SetFloat("mixerVolume", volume);
        GlobalSettings.setVolume(volume);
    }

    public void changeQualityLevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
        toggleVSync(VSyncToggle.isOn);
    }

    public void changeResolution(int resolutionIndex)
    {
        Resolution screenResolution = resolutions[resolutionIndex];
        Screen.SetResolution(screenResolution.width, screenResolution.height, fullScreenToggle.isOn);
    }

    public void changeFullscreen(bool toggle)
    {
        Screen.fullScreen = toggle;
    }

    public void toggleVSync(bool toggle)
    {
        if (toggle)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public void setSensitivity(float sensitivity)
    {
        player.setSensitivity(sensitivity);
        GlobalSettings.setSensitivity(sensitivity);
    }

    public void goToMainMenu()
    {
        resume();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }

    public void setFov(float fieldOfView)
    {
        player.setFov(fieldOfView);
        GlobalSettings.setFov(fieldOfView);
    }
}
