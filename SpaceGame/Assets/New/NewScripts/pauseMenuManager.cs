using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenuManager : MonoBehaviour
{
    public PauseMenu PauseMenu;

    public void Start()
    {
        PauseMenu = GetComponent<PauseMenu>();
    }
    public void Resume()
    {
        PauseMenu.isPaused = false;
        PauseMenu. pauseMenuUi.SetActive(false);
        StateSwitch.SwitchState(GameStateManager.Instance.state = GameStateManager.gameState.inGame);
    }
    public void Settings()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }

}
