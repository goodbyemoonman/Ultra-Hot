using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INPUTTYPE { NONE, MOUSE, KEYBOARD }

public class TimeScaleManager : MonoBehaviour {
    readonly float scaler = 0.03f;
    bool canScale = true;
    float target;

    private void Start()
    {
        StartCoroutine(ControlTimeScale());
    }

    IEnumerator ControlTimeScale()
    {
        target = 0.05f;
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

    public void SetInputType(INPUTTYPE input)
    {
        switch (input)
        {
            case INPUTTYPE.NONE:
                target = 0.05f;
                break;
            case INPUTTYPE.MOUSE:
                target = 0.25f;
                break;
            case INPUTTYPE.KEYBOARD:
                target = 1f;
                break;
        }
    }

    public void SetTimeScaleImmediately(float to)
    {
        Time.timeScale = Mathf.Clamp01(to);
    }
    
    public void SetScaleSwitch(bool isTurnOn)
    {
        canScale = isTurnOn;
    }

    public void FixTimeScale(float scale, float duration)
    {
        Time.timeScale = scale;
        canScale = false;

        StopAllCoroutines();
        StartCoroutine(TurnOnTimer(duration));
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), Time.timeScale.ToString());
    }

    IEnumerator TurnOnTimer(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        canScale = true;
    }
}