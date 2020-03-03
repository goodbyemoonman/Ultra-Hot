using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour {
    protected float cooltime = 1;
    protected float range = 1;
    bool isCooldown = false;
    protected int targetLayer;
    protected int parentLayer;

    public void Init()
    {
        isCooldown = false;
    }

    public virtual void TryExecute()
    {
        if(isCooldown)
        {
            return;
        }
        isCooldown = true;
        StopAllCoroutines();
        StartCoroutine(Timer(cooltime));

        if (parentLayer == LayerMask.NameToLayer("PlayerCharacter"))
            Execute();
        else
            ExecuteEnemy();
    }

    protected abstract void Execute();

    protected abstract void ExecuteEnemy();
    
    void RefreshCooldown()
    {
        isCooldown = false;
    }

    public abstract void ThrowThisObj();

    IEnumerator Timer(float duration)
    {
        if (parentLayer == LayerMask.NameToLayer("PlayerCharacter"))
            yield return new WaitForSecondsRealtime(duration);
        else
            yield return new WaitForSeconds(duration);
        RefreshCooldown();
    }
    
    public void SetTargetLayer(int layer)
    {
        targetLayer = layer;
    }

    public void SetLayer(int layer)
    {
        parentLayer = layer;
    }
}
