using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour {
    TimeState accelState = new AccelTimeState();
    TimeState decelState = new DecelTimeState();
    TimeState timeState;

    public bool canScale = true;
    float goal;

    private void Start()
    {
        timeState = decelState;
        StartCoroutine(ControlTimeScale());
    }

    IEnumerator ControlTimeScale()
    {
        while (canScale)
        {
            timeState.Update(goal);
            yield return new WaitForSecondsRealtime(0.025f);
        }
    }

    public void SetTimeScale(float to)
    {
        goal = to;
        if (Time.timeScale < to)
        {
            timeState = accelState;
        }
        else
            timeState = decelState;
    }

    public void SetTimeScale(float to, bool isEmergency)
    {
        if (isEmergency)
        {
            Time.timeScale = to;
            return;
        }
        else
            SetTimeScale(to);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), Time.timeScale.ToString());
    }
}

abstract class TimeState
{
    protected readonly float MINTIMESCALE = 0.1f;

    public abstract void Update(float goal);
    protected float GetBig(float a, float b)
    {
        return (a < b) ? b : a;
    }
}

class AccelTimeState : TimeState
{ 
    public override void Update(float goal)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale + 0.0125f, MINTIMESCALE, GetBig(goal, 1f));
    }

}

class DecelTimeState : TimeState
{
    public override void Update(float goal)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale - 0.025f, GetBig(MINTIMESCALE, goal), 1f);
    }
}

class NullTimeState : TimeState
{
    public override void Update(float goal)
    {

    }
}