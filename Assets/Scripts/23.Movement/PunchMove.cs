using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchMove : MonoBehaviour {
    Vector2 originPos;
    TrailRenderer tr;
    RaycastHit2D[] hits;
    Transform parent;
    int targetLayer;

    private void Awake()
    {
        parent = transform.parent;
        parent.GetComponent<HealthManager>().CharaStateTeller += StateObserver;
        if (parent.CompareTag("Player"))
            targetLayer = (1 << LayerMask.NameToLayer("EnemyCharacter"));
        else
            targetLayer = (1 << LayerMask.NameToLayer("PlayerCharacter"));
        
        tr = GetComponent<TrailRenderer>();
        originPos = transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localPosition = originPos;
        StartCoroutine(Move());
        SoundManager.Instance.PlaySE(gameObject, AudioClipList.Swing1, AudioClipList.Swing2, AudioClipList.Swing3);
    }

    IEnumerator Move()
    {
        hits = null;
        tr.Clear();
        Vector2 origin = transform.position;
        Vector2 target = Vector2.right;
        for(float t = 0; t < 0.1f; )
        {
            transform.localPosition = Vector2.Lerp(originPos , target, t * 10);
            if (parent.CompareTag("Player"))
            {
                t += 0.02f;
                yield return new WaitForSecondsRealtime(0.02f);
            }
            else
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        hits = Physics2D.RaycastAll(origin, ((Vector2)transform.position - origin),
            ((Vector2)transform.position - origin).magnitude, targetLayer);
        //Debug.DrawLine(origin, (Vector2)transform.position, Color.blue, 0.5f);

        foreach(RaycastHit2D hit in hits)
        {
            hit.collider.SendMessage("GetDamaged", 1);
        }

        for(float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            transform.localPosition = Vector2.Lerp(target, originPos * -1, t);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    void StateObserver(HealthManager.CharacterState state)
    {
        if(state == HealthManager.CharacterState.Sturn)
            gameObject.SetActive(false);
    }
}