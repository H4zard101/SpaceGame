using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void EndGame()
    {
        Application.Quit();
    }
    public void SettingButton()
    {
        // logic for settings
        Debug.Log("Settings Button Pressed");
    }
    public void CreditsButton()
    {
        // logic for credits
        Debug.Log("Credits Button Pressed");
    }
}
