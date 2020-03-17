using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour {
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    public void GameStateObserver(GameStateList state)
    {
        if(state == GameStateList.Defeat || state == GameStateList.Win)
        {
            StageManager.Instance.GameStateTeller -= GameStateObserver;
            ps.Stop();
        }
    }

    public void OnParticleSystemStopped()
    {
        ObjPoolManager.Instance.ReturnObject(gameObject
);
    }

}
