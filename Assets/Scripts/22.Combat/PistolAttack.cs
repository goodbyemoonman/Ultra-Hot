using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAttack : AttackBase {
    public Transform firePos;
    public GameObject bulletPrefab;
    int targetLayer;
    int bulletCount = 6;

    private void Awake()
    {
        cooltime = 1f;
    }

    public void EquipTo(GameObject parent)
    {
        transform.SetParent(parent.transform);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        if (parent.layer == LayerMask.NameToLayer("PlayerCharacter"))
            targetLayer = 1 << LayerMask.NameToLayer("EnemyCharacter");
        else
            targetLayer = 1 << LayerMask.NameToLayer("PlayerCharacter");

        targetLayer |= 1 << LayerMask.NameToLayer("Wall");
    }

    public override void ThrowThisObj()
    {
        float cos = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        float sin = Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
        Vector2 dir = new Vector2(cos, sin);
        RaycastHit2D hit = Physics2D.Raycast(firePos.position, dir, 5f, targetLayer);
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
