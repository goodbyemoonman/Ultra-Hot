using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateObserver : MonoBehaviour {

    private void OnEnable()
    {
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageReady:
                StageManager.Instance.GameStateTeller -= GameStateObserver;
                ObjPoolManager.Instance.ReturnObject(gameObject);
                break;
        }
    }
}
