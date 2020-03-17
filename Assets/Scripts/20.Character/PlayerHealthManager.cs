using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : HealthManager {
    bool isPlaying = false;

    private new void Awake()
    {
        base.Awake();
        isPlaying = false;
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageStart:
                isPlaying = true;
                break;
            default:
                isPlaying = false;
                break;
        }
    }

    public override void GetDamaged(int damage)
    {
        if (isPlaying)
        {
            isPlaying = false;
            StageManager.Instance.DefeatStage();
        }
    }
}
