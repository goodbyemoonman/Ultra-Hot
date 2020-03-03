using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
    Rigidbody2D rgbd;
    public delegate void Remind(int n);
    public event Remind BulletReminder;
    AttackBase ab;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        ab = GetComponent<AttackBase>();
    }

    public void EquipTo(Transform parent)
    {
        transform.SetParent(parent);
        StopAllCoroutines();
        StartCoroutine(EquipMovement());
        ab.Init();
        TargetLayerSet(parent.gameObject.layer);
        ab.SetLayer(parent.gameObject.layer);
    }

    void TargetLayerSet(int parentLayer)
    {
        int targetLayer;

        if (parentLayer == LayerMask.NameToLayer("PlayerCharacter"))
            targetLayer = 1 << LayerMask.NameToLayer("EnemyCharacter");
        else
            targetLayer = 1 << LayerMask.NameToLayer("PlayerCharacter");

        targetLayer |= 1 << LayerMask.NameToLayer("Wall");
        ab.SetTargetLayer(targetLayer);
    }

    public void Drop()
    {
        transform.SetParent(null);
        StartCoroutine(DropMovement());
    }

    IEnumerator EquipMovement()
    {
        float t = 0;
        while (t < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, t * 10);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, t * 10);
            t += 0.02f;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    IEnumerator DropMovement()
    {
        rgbd.AddForce(new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * 5f);
        yield return null;
    }

    public void ThrowHelper(Vector3 targetPos, Vector2 dir, bool isBlocked)
    {
        StopAllCoroutines();
        StartCoroutine(ThrowMovement(targetPos, dir, isBlocked));
    }

    IEnumerator ThrowMovement(Vector3 targetPos, Vector2 dir, bool isBlocked)
    {
        gameObject.layer = LayerMask.NameToLayer("EquipItem");
        Vector3 originPos = transform.position;
        for (float t = 0; t < 1; t += 0.2f)
        {
            transform.position = Vector3.Lerp(originPos, targetPos, t);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        transform.SetParent(null);
        rgbd.WakeUp();
        if (isBlocked)
            Drop();
        else
            rgbd.AddForce(dir.normalized * 100f);
    }

    public void BulletRemind(int n)
    {
        Debug.Log("bullet remind : " + n.ToString());
        if (BulletReminder != null)
            BulletReminder(n);
    }
}
