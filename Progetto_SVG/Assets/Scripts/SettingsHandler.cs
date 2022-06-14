using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsHandler : MonoBehaviour
{
    public AudioMixer mixer;
    public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown qualitySelector;
    public TMP_Dropdown resolutionSelector;
    public Toggle fullScreenToggle;
    public PlayerController player;

    private bool isActive = false;
    private Canvas settingsMenu;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionSelector.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }

        }

        resolutionSelector.AddOptions(resolutionOptions);
        resolutionSelector.value = currentResolutionIndex;
        resolutionSelector.RefreshShownValue();

        qualitySelector.value = QualitySettings.GetQualityLevel();
    }

    void Awake() {
        settingsMenu = GetComponent<Canvas>();
        settingsMenu.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape") && !isActive) pause();
    }

    void pause() {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        settingsMenu.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        isActive = true;
        player.enterMenu();
    }

    public void resume() {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        settingsMenu.enabled = false;
        isActive = false;
        player.exitMenu();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void changeVolume(float volume) {
        mixer.SetFloat("mixerVolume", volume);
    }

    public void changeLevel(int value) {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }

    public void changeResolution(int resolutionIndex) {
        Resolution screenResolution = resolutions[resolutionIndex];
        Screen.SetResolution(screenResolution.width, screenResolution.height, fullScreenToggle.isOn);
    }

    public void changeFullscreen(bool toggle) {
        fullScreenToggle.isOn = toggle;
        Screen.fullScreen = toggle;
    }

    public void setSensitivity(float sensitivity) {
        player.setSensitivity(sensitivity);
    }

    public void goToMainMenu() {
        resume();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
