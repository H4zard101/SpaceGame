using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public GameObject gameplayOptions;
    public GameObject audioOptions;
    public GameObject graphicsOptios;
    public GameObject controlsOption;

    public void Start()
    {
        //gameplayOptions = GameObject.Find("GameplaySettingsLayout");
        //audioOptions = GameObject.Find("AudioSettingsLayout");
        //graphicsOptios = GameObject.Find("GraphicsSettingsLayout");
        //controlsOption = GameObject.Find("ControlsSettingsLayout");
        
        gameplayOptions.SetActive(false);
        controlsOption.SetActive(false);
        audioOptions.SetActive(false);
        graphicsOptios.SetActive(false);
    }

    public void GameplaySetting()
    {
        gameplayOptions.SetActive(true);
        controlsOption.SetActive(false);
        audioOptions.SetActive(false);
        graphicsOptios.SetActive(false);
    }
    public void AudioSetting()
    {
        gameplayOptions.SetActive(false);
        controlsOption.SetActive(false);
        audioOptions.SetActive(true);
        graphicsOptios.SetActive(false);
    }
    public void GraphicsSetting()
    {
        gameplayOptions.SetActive(false);
        controlsOption.SetActive(false);
        audioOptions.SetActive(false);
        graphicsOptios.SetActive(true);
    }
    public void ControlSetting()
    {
        gameplayOptions.SetActive(false);
        controlsOption.SetActive(true);
        audioOptions.SetActive(false);
        graphicsOptios.SetActive(false);
    }
}
