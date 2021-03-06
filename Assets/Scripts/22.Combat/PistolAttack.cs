﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAttack : AttackBase {
    public Transform firePos;
    int bulletCount = 6;
    Equipment e;

    private void Awake()
    {
        e = GetComponent<Equipment>();
        range = 5f;
        cooltime = 0.5f;
    }

    private void OnEnable()
    {
        bulletCount = 6;
        Init();
    }

    public override void ThrowThisObj()
    {
        float cos = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        float sin = Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 dir = new Vector2(cos, sin);
        RaycastHit2D hit = Physics2D.Raycast(firePos.position, dir, 2f, targetLayer);
        //Debug.DrawLine(firePos.position, ((Vector2)firePos.position + (dir * 2f)), Color.blue, 1f);
        Vector3 targetPos;

        if (hit)
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Wall"))
                hit.collider.gameObject.SendMessage("GetDamaged", 1);
            targetPos = hit.point;
        }
        else
        {
            targetPos = dir * 3f + (Vector2)firePos.position;
        }
        StopAllCoroutines();
        e.ThrowHelper(targetPos);
    }
    
    protected override void Execute()
    {
        if (!HasBullet())
            return;

        ShootBullet(Vector3.zero);
    }

    protected override void ExecuteEnemy()
    {
        if (!HasBullet())
        {
            gameObject.SendMessage("RunOutBullet");
            return;
        }
        ShootBullet(new Vector3(0, 0, Random.Range(-15f, 15f)));
    }

    bool HasBullet()
    {
        bool result;
        if(bulletCount < 1)
        {
            result = false;
        }
        else
        {
            bulletCount--;
            result = true;
        }
        e.BulletRemind(bulletCount);
        return result;
    }

    void ShootBullet(Vector3 addRotation)
    {
        SoundManager.Instance.PlaySE(gameObject, AudioClipList.Shoot01, AudioClipList.Shoot02);
        GameObject bullet = ObjPoolManager.Instance.GetObject(ObjectPoolList.BulletPrefab);
        bullet.transform.SetPositionAndRotation(
            firePos.position,
            Quaternion.Euler(transform.eulerAngles + addRotation));
        bullet.transform.SetParent(null);
        bullet.SetActive(true);
    }

    public override bool EnoughBullet()
    {
        return (bulletCount > 0);
    }
}
