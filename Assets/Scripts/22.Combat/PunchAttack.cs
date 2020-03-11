using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAttack : AttackBase {
    public GameObject punchR;
    public GameObject punchL;
    bool sw;

    private void Awake()
    {
        sw = false;
        cooltime = 0.5f;
        range = 0.8f;
    }

    protected override void Execute()
    {
        if (sw)
        {
            punchL.SetActive(true);
            sw = false;
        }
        else
        {
            punchR.SetActive(true);
            sw = true;
        }
    }

    public override void ThrowThisObj()
    {
    }

    protected override void ExecuteEnemy()
    {
        if (sw)
        {
            punchL.SetActive(true);
            sw = false;
        }
        else
        {
            punchR.SetActive(true);
            sw = true;
        }
    }

    public override bool EnoughBullet()
    {
        return true;
    }
}
