using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwitch : MonoBehaviour
{
    public static void SwitchState(GameStateManager.gameState state)
    {
        GameStateManager.Instance.state = state;
        
    }
}
