using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    public GameObject MainMenuPanel;


    public void Start()
    {
        SettingsPanel = GameObject.Find("SettingsPage");
        CreditsPanel = GameObject.Find("CreditsParent");
        MainMenuPanel = GameObject.Find("MainMenuParent");

        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if(GameStateManager.Instance != null)
        {
            StateSwitch.SwitchState(GameStateManager.Instance.state = GameStateManager.gameState.inGame);
        }
    }
    public void EndGame()
    {
        Application.Quit();
    }
    public void SettingButton()
    {
        SettingsPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }
    public void CreditsButton()
    {
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }
    public void MainMenu()
    {
        SettingsPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }
}
