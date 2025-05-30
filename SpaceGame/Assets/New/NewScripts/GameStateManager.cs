using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
 
    public static GameStateManager Instance;


    public enum gameState
    {
        none,
        menu,
        inGame,
        death,
        pause
          
    }

    public gameState state;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        state = gameState.menu;
    }
}
