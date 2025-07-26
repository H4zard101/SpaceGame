using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuNav : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [SerializeField] private Button loadingButton;
    [SerializeField] private TMP_Text loadingText;
    public GameObject bottomBar;



    public void Start()
    {
        gameState = GameObject.Find("GameStateManager").GetComponent<GameState>();

        if (!DataPersistenceManager.instance.HasGameData())
        {
            loadingButton.interactable = false;
            loadingText.color = new Color(12f / 255f, 58f / 255f, 58f / 255f, 255f / 255f);

        }

        else
        {
            loadingButton.interactable = true;
            loadingText.color = new Color(155f / 255f, 255f / 255f, 254f / 255f, 255f / 255f);
        }
    }
    public void StartGame()
    {
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Save_Load2;

        Debug.Log("Save/Load State");
        gameState.LoadSaveLoad2StateUI();

    }

    public void Settings()
    {
        gameState.GetComponent<GameState>().game_state = GameState.gameState.Settings;
        Debug.Log("Settings State");
        gameState.LoadSettingsStateUI();

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
}
