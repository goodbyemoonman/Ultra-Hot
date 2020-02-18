using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunchAttack : AttackBase {
    public GameObject punchR;
    public GameObject punchL;

    private void Awake()
    {
        cooltime = 0.2f;
    }

    protected override void Execute()
    {
        if (punchR.activeInHierarchy)
            punchL.SetActive(true);
        else
            punchR.SetActive(true);
    }

    public override void ThrowThisObj()
    {
    }
}
