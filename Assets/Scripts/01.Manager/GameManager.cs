using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMESTATE { STAGE_LOAD, MAP_LOAD, MAP_READY, STAGE_BEGIN }

public class GameManager : MonoBehaviour {
    GAMESTATE gameStateNow = GAMESTATE.STAGE_LOAD;
    public delegate void GameStateDelegate(GAMESTATE gameState);
    public event GameStateDelegate GameStateObserver;
    
    public void SetGameState(GAMESTATE state)
    {
        if (state != gameStateNow)
        {
            GameStateObserver(state);
            gameStateNow = state;
        }
    }
}
