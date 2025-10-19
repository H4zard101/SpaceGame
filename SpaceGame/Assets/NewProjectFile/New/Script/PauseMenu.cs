using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUi;
    public bool isPaused;

    public GameState gameState;
    public void Start()
    {
        //pauseMenuUi = GameObject.Find("pause"); // find the object and set it to the variable
        pauseMenuUi.SetActive(false); // set it to false on start so it doesnt appear on the screen

        // ASSIGN THE GAME STATE OBJECT
        gameState = GameObject.Find("GameStateManager").GetComponent<GameState>();
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
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Game_Menu;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuUi.SetActive(false);
        gameState.GetComponent<GameState>().game_state = GameState.gameState.InGame;
        isPaused = false;
    }
    public void SaveButton()
    {
        DataPersistenceManager.instance.SaveGame();
        Debug.Log("SavedGame");
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
    public void SettingButton()
    {
        // TO Do Later
    }

}
