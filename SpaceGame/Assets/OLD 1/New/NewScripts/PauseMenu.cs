using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUi;
    public bool isPaused;


    public void Start()
    {
        pauseMenuUi = GameObject.Find("PauseMenu"); // find the object and set it to the variable
        pauseMenuUi.SetActive(false); // set it to false on start so it doesnt appear on the screen
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuUi.SetActive(true);
        StateSwitch.SwitchState(GameStateManager.Instance.state = GameStateManager.gameState.pause);
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuUi.SetActive(false);
        StateSwitch.SwitchState(GameStateManager.Instance.state = GameStateManager.gameState.inGame);
        isPaused = false;
    }
}
