using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAttack : AttackBase {
    public GameObject punchR;
    public GameObject punchL;

    private void Awake()
    {
        cooltime = 0.2f;
        range = 1.5f;
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

    protected override void ExecuteEnemy()
    {
        if (punchL.activeInHierarchy)
            punchR.SetActive(true);
        else
            punchL.SetActive(true);
    }
}
