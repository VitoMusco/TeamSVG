using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class SettingsHandler : MonoBehaviour
{

    public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown qualitySelector;
    public TMP_Dropdown resolutionSelector;

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

    public void ChangeLevel(int value) {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }

    public void ChangeResolution(int resolutionIndex) {
        Resolution screenResolution = resolutions[resolutionIndex];
        Screen.SetResolution(screenResolution.width, screenResolution.height, Screen.fullScreen);
    }
}
