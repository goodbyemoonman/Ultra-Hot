using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
    Transform tf;

    private void Awake()
    {
        tf = transform.parent;
    }

    public void EquipTo(GameObject parent)
    {
        int targetLayer;
        tf.SetParent(parent.transform);
        StopAllCoroutines();
        StartCoroutine(EquipMovement());
        if (parent.layer == LayerMask.NameToLayer("PlayerCharacter"))
            targetLayer = 1 << LayerMask.NameToLayer("EnemyCharacter");
        else
            targetLayer = 1 << LayerMask.NameToLayer("PlayerCharacter");

        targetLayer |= 1 << LayerMask.NameToLayer("Wall");

        SendMessage("SetTargetLayer", targetLayer);
    }

    public void Drop()
    {
        tf.SetParent(null);
        StartCoroutine(DropMovement());
    }

    IEnumerator EquipMovement()
    {
        float t = 0;
        while (t < 0.1f)
        {
            tf.localPosition = Vector3.Lerp(tf.localPosition, Vector3.zero, t * 5);
            tf.localEulerAngles = Vector3.Lerp(tf.localEulerAngles, Vector3.zero, t * 5);
            t += 0.02f;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        tf.localPosition = Vector3.zero;
        tf.localEulerAngles = Vector3.zero;
    }

    IEnumerator DropMovement()
    {
        float t = 0;
        Vector3 targetPos = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        Vector3 originPos = tf.position;

        while (t < 1f)
        {
            tf.position = Vector3.Lerp(originPos, targetPos, t);
            yield return null;
            t += Time.deltaTime;
        }
    }


}
