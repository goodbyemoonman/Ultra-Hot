using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INPUTTYPE { NONE, MOUSE, KEYBOARD };

public class TimeScaleManager : MonoBehaviour {
    readonly float scaler = 0.03f;
    public bool canScale = true;
    float target;

    private void Start()
    {
        StartCoroutine(ControlTimeScale());
    }

    IEnumerator ControlTimeScale()
    {
        target = 0;

        while (canScale)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, target, scaler);
            yield return new WaitForSecondsRealtime(0.025f);
        }
    }

    public void SetInputType(INPUTTYPE input)
    {
        switch (input)
        {
            case INPUTTYPE.NONE:
                target = 0.1f;
                break;
            case INPUTTYPE.MOUSE:
                target = 0.25f;
                break;
            case INPUTTYPE.KEYBOARD:
                target = 1f;
                break;
        }
    }

    public void SetTimeScaleimmediately(float to)
    {
        Time.timeScale = Mathf.Clamp01(to);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), Time.timeScale.ToString());
    }
}