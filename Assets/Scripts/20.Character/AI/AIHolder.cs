using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHolder : MonoBehaviour {
    public Transform tf;
    WalkAroundAI walkAroundAI;
    SeekAI seekAI;
    AIBase AINow;

    private void Awake()
    {
        walkAroundAI = new WalkAroundAI();
        walkAroundAI.Initialize(gameObject);
        walkAroundAI.SetHoler(this);
        seekAI = new SeekAI();
        seekAI.Initialize(gameObject);
        seekAI.SetHoler(this);
    }

    private void OnEnable()
    {
        Seek(tf);
    }

    private void Update()
    {
        AINow.Do(gameObject);
    }

    public void Seek(Transform tf)
    {
        seekAI.SetTargetTf(tf);
        AINow = seekAI;
    }

    public void RunOutBullet()
    {
        gameObject.SendMessage("Throw");
    }

    public void Arrive()
    {
        seekAI.Arrive(gameObject);
    }
}
