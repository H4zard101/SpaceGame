using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuNav : MonoBehaviour
{

    [Header("Menu Navigation")]
    [SerializeField] private GameState gameState;
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [SerializeField] private Button loadingButton;
    [SerializeField] private TMP_Text loadingText;
    public GameObject bottomBar;



    public void Start()
    {
        // ASSIGN THE GAME STATE OBJECT
        gameState = GameObject.Find("GameStateManager").GetComponent<GameState>();

        // CHECK TO SEE IF THERE IS DATA FOR THE GAME, IF NOT DISABLE THE LOAD GAME OPTION
        if (!DataPersistenceManager.instance.HasGameData())
        {
            loadingButton.interactable = false;
            loadingText.color = new Color(12f / 255f, 58f / 255f, 58f / 255f, 255f / 255f);

        }
        // IF THERE IS GAME DATA THEN ENABLE THE LOAD GAME OPTION
        else
        {
            loadingButton.interactable = true;
            loadingText.color = new Color(155f / 255f, 255f / 255f, 254f / 255f, 255f / 255f);
        }
    }
    public void MainMenu()
    {
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Main_Menu;
        gameState.LoadMainMenuUI();

    }
    public void SaveLoad()
    {
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Save_Load;
        gameState.LoadSaveLoadUI();
    }

    public void Settings()
    {
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Settings;
        gameState.LoadSettingsUI();

    }

    public void Credits()
    {
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Credits;
        gameState.LoadCreditsUI();
    }

    public void Quit()
    {
        Application.Quit();
    }


    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true); 

    }
    public void DeactivateMenu() 
    {
        this.gameObject.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (GameStateManager.Instance != null)
        {
            StateSwitch.SwitchState(GameStateManager.Instance.state = GameStateManager.gameState.inGame);
        }
    }
}
