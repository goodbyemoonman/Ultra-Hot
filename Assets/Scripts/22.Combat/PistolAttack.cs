using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAttack : AttackBase {
    public Transform firePos;
    Transform tf;
    public GameObject bulletPrefab;
    int bulletCount = 6;

    private void Awake()
    {
        tf = transform.parent;
        cooltime = 1f;
    }
    
    public override void ThrowThisObj()
    {
        float cos = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        float sin = Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 dir = new Vector2(cos, sin);
        RaycastHit2D hit = Physics2D.Raycast(firePos.position, dir, 3f, targetLayer);
        if (hit && hit.collider.gameObject.layer == targetLayer)
        {
            hit.collider.SendMessage("GetDamaged", 1);
        }
    }

    protected override void Execute()
    {
        if (bulletCount < 1)
            return;

        bulletCount--;

        GameObject bullet = ObjPoolManager.Instance.GetObject(ObjectPoolList.BulletPrefab);
        bullet.transform.SetPositionAndRotation(
            firePos.position, 
            Quaternion.Euler(transform.eulerAngles));
        bullet.transform.SetParent(null);
        bullet.SetActive(true);
    }

}
