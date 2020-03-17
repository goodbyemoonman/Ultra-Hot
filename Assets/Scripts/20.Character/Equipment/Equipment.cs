using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
    Rigidbody2D rgbd;
    CircleCollider2D col2d;
    public delegate void Remind(int n);
    public event Remind BulletReminder;
    AttackBase ab;
    GameObject eKeyUi;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        ab = GetComponent<AttackBase>();
        col2d = GetComponent<CircleCollider2D>();
        StopAllCoroutines();
        eKeyUi = transform.GetChild(1).gameObject;
        eKeyUi.SetActive(true);
    }

    public void EquipTo(Transform parent)
    {
        rgbd.Sleep();
        col2d.enabled = false;
        eKeyUi.SetActive(false);
        transform.SetParent(parent);
        StopAllCoroutines();
        StartCoroutine(EquipMovement());
        ab.Init();
        TargetLayerSet(parent.CompareTag("Player"));
        ab.PlayerSet(parent.CompareTag("Player"));
        SoundManager.Instance.PlaySE(gameObject, AudioClipList.Equip);
    }

    void TargetLayerSet(bool isPlayer)
    {
        int targetLayer;

        if (isPlayer)
            targetLayer = Utility.EnemyLayer;
        else
            targetLayer = Utility.PlayerLayer;

        targetLayer |= Utility.WallLayer;
        ab.SetTargetLayer(targetLayer);
    }

    public void DestroyThis()
    {
        transform.SetParent(null);
        GameObject effect = ObjPoolManager.Instance.GetObject(ObjectPoolList.PistolDistroyEffect);
        effect.transform.position = transform.position;
        effect.SetActive(true);
        StageManager.Instance.GameStateTeller -= GameStateObserver;
        ObjPoolManager.Instance.ReturnObject(gameObject);
    }

    public void Drop()
    {
        DropMovement();
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

    void DropMovement()
    {
        transform.SetParent(null);
        col2d.enabled = true;
        rgbd.WakeUp();
        rgbd.AddForce(new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * 5f);
        transform.eulerAngles = Vector3.zero;
        eKeyUi.SetActive(true);
    }

    public void ThrowHelper(Vector3 targetPos)
    {
        StopAllCoroutines();
        StartCoroutine(ThrowMovement(targetPos));
    }

    IEnumerator ThrowMovement(Vector3 targetPos)
    {
        Vector3 originPos = transform.position;
        for (float t = 0; t < 1; t += 0.2f)
        {
            transform.position = Vector3.Lerp(originPos, targetPos, t);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        transform.SetParent(null);
        rgbd.WakeUp();

        DestroyThis();
    }

    public void BulletRemind(int n)
    {
        if (BulletReminder != null)
            BulletReminder(n);
    }

    private void OnEnable()
    {
        StageManager.Instance.GameStateTeller += GameStateObserver;
        transform.eulerAngles = Vector3.zero;
        rgbd.WakeUp();
        col2d.enabled = true;
        eKeyUi.SetActive(true);
        transform.SetParent(null);
    }

    public void GameStateObserver(GameStateList state)
    {
        if (state == GameStateList.Win || state == GameStateList.Defeat)
        {
            ThrowHelper(transform.position);
        }
    }
}
