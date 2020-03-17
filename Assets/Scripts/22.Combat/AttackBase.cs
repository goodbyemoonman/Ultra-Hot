using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour {
    protected float cooltime = 1;
    protected float range = 1;
    bool isCooldown = false;
    protected int targetLayer;
    protected bool isUsedPlayer;

    public void Init()
    {
        isCooldown = true;
        StartCoroutine(Timer(0.5f));
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

        if (isUsedPlayer)
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
        if (isUsedPlayer)
            yield return new WaitForSecondsRealtime(duration);
        else
            yield return new WaitForSeconds(duration * 1.5f);
        RefreshCooldown();
    }
    
    public void SetTargetLayer(int layer)
    {
        targetLayer = layer;
    }

    public void PlayerSet(bool isIt)
    {
        isUsedPlayer = isIt;
    }

    public float AtkRange { get { return range; } }

    public abstract bool EnoughBullet();
}
