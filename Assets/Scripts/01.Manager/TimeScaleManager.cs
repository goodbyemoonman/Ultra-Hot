using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INPUTTYPE { NONE, MOUSE, KEYBOARD }

public class TimeScaleManager : MonoBehaviour {
    readonly float scaler = 0.01f;
    readonly float floor = 0.05f;
    bool canScale = true;
    float target;

    private void Awake()
    {
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    IEnumerator ControlTimeScale()
    {
        target = floor;
        int multiplier = 1;
        while (true)
        {
            if (Time.timeScale > target)
                multiplier = 2;
            else
                multiplier = 1;
            if (canScale)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, target, scaler * multiplier);
            }
            yield return new WaitForSecondsRealtime(0.025f);
        }
    }

    public void ActTimeScale()
    {
        Time.timeScale = Mathf.Clamp01(Time.timeScale + 0.15f);
    }

    public void SetInputType(INPUTTYPE input)
    {
        switch (input)
        {
            case INPUTTYPE.NONE:
                target = floor;
                break;
            case INPUTTYPE.MOUSE:
                target = 0.25f;
                break;
            case INPUTTYPE.KEYBOARD:
                target = 1f;
                break;
        }
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageStart:
                Time.timeScale = 0.5f;
                canScale = true;
                StartCoroutine(ControlTimeScale());
                break;
            default:
                StopAllCoroutines();
                Time.timeScale = 1f;
                canScale = false;
                break;
        }
    }
}