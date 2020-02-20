using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour {
    protected float cooltime = 1;
    protected float range = 1;
    bool isCooldown = false;

    public virtual void TryExecute()
    {
        if(isCooldown)
        {
            return;
        }
        isCooldown = true;
        StopAllCoroutines();
        StartCoroutine(Timer(cooltime));

        Execute();
    }

    protected abstract void Execute();

    void RefreshCooldown()
    {
        isCooldown = false;
    }

    public abstract void ThrowThisObj();

    IEnumerator Timer(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        RefreshCooldown();
    }
}
