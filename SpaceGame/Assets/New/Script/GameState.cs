using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject Save_LoadUI2;
    public enum gameState 
        {
            Main_Menu, 
            Save_Load2, // SAVE AND LOAD SCREEN WITH THE SAVE SLOTS 
            Settings,
            InGame,
            Game_Menu
        }

    public gameState game_state;

    public void Start()
    {
        game_state = gameState.Main_Menu;
        Debug.Log("MainMenu State");
        Save_LoadUI2.SetActive(false);
    }


    // functions to load game state UI Called in the Menu Nav script
    public void LoadMainMenuStateUI()
    {
        MainMenuUI.SetActive(true);
        Save_LoadUI2.SetActive(false);
    }
    public void LoadSaveLoad2StateUI()
    {
        MainMenuUI.SetActive(false);
        Save_LoadUI2.SetActive(true);
    }
    public void LoadSettingsStateUI()
    {

    }
}
