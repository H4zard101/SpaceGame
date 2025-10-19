using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

    // PARENT GAME OBJECTS FOR UI ELEMENT
    [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject Save_LoadUI;
    [SerializeField] private GameObject SettingsUI;
    [SerializeField] private GameObject CreditsUI;
    public enum gameState 
        {
            Main_Menu, // THE GAME WILL START WITH THIS 
            Save_Load, // SAVE AND LOAD SCREEN WITH THE SAVE SLOTS 
            Settings, // FROM THE MAIN MENU SCREEN
            Credits, // FROM THE MAIN MENU SCREEN
            InGame, // GAME
            Game_Menu // GAME PAUSE MENU
        }

    public gameState game_state;

    public void Start()
    {
        // SET THE GAME STATE
        game_state = gameState.Main_Menu;

        // SET THE UI ELEMENTS
        SettingsUI = GameObject.Find("SettingsPage");
        CreditsUI = GameObject.Find("CreditsParent");
        MainMenuUI = GameObject.Find("MainMenuParent");
        Save_LoadUI = GameObject.Find("Parent");

        // DEFUALT ALL TO BE FALSE EXEPT THE MAIN MENU
        MainMenuUI.SetActive(true);
        SettingsUI.SetActive(false);
        CreditsUI.SetActive(false);
        Save_LoadUI.SetActive(false);

    }


    // FUNCTIONS TO BE CALLED THAT ENABLES THE UI ELEMENTS
    public void LoadMainMenuUI()
    {
        MainMenuUI.SetActive(true);
        Save_LoadUI.SetActive(false);
        CreditsUI.SetActive(false);
        SettingsUI.SetActive(false);
    }
    public void LoadSaveLoadUI()
    {
        MainMenuUI.SetActive(false);
        Save_LoadUI.SetActive(true);
        CreditsUI.SetActive(false);
        SettingsUI.SetActive(false);
    }

    public void LoadSettingsUI()
    {
        MainMenuUI.SetActive(false);
        Save_LoadUI.SetActive(false);
        CreditsUI.SetActive(false);
        SettingsUI.SetActive(true);
    }
    public void LoadCreditsUI()
    {
        MainMenuUI.SetActive(false);
        Save_LoadUI.SetActive(false);
        CreditsUI.SetActive(true);
        SettingsUI.SetActive(false);
    }
}
