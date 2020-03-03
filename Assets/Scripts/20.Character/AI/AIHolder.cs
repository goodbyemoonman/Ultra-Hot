using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHolder : MonoBehaviour {
    WalkAroundAI walkAroundAI;
    SeekAI seekAI;
    AIBase AINow;

    private void Awake()
    {
        walkAroundAI = new WalkAroundAI();
        walkAroundAI.Awake();
        walkAroundAI.Initialize(gameObject);
        walkAroundAI.SetHoler(this);
        seekAI = new SeekAI();
        seekAI.Awake();
        seekAI.Initialize(gameObject);
        seekAI.SetHoler(this);
    }

    private void Update()
    {
        walkAroundAI.Do(gameObject);
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

}
