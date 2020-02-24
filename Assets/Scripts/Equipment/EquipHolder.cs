using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHolder : MonoBehaviour {
    float boundary;
    RaycastHit2D[] hits;
    int targetLayer;
    GameObject equipment;
    public Transform equipPos;
    FixedJoint2D fxjt2d;

    private void Awake()
    {
        fxjt2d = GetComponent<FixedJoint2D>();
        targetLayer = 1 << LayerMask.NameToLayer("EquipItem");
        boundary = 1.5f;
    }

    public bool IsEquipSomethig()
    {
        if (equipment == null)
            return false;
        else
            return true;
    }

    bool CheckItem(float boundary)
    {
        hits = Physics2D.CircleCastAll(
            transform.position, 
            boundary, Vector2.zero, 0f, targetLayer);
        if (hits.Length != 0)
            return true;
        else
            return false;
    }

    public void EKeyDown()
    {
        if (CheckItem(boundary) == false)
            return;

        GameObject equipCandidate = GetShortestDistanceGO(boundary);

        if (equipCandidate == null)
            return;
        fxjt2d.enabled = true;
        fxjt2d.connectedBody = equipCandidate.GetComponent<Rigidbody2D>();
        equipCandidate.SendMessage("EquipTo", equipPos);
        equipment = equipCandidate;
    }

    GameObject GetShortestDistanceGO(float boundary)
    {
        GameObject result = null;
        float shortest = boundary + 1;
        for (int i = 0; i < hits.Length; i++)
        {
            float distance = GetDistance(hits[i].collider.transform);
            if (shortest > distance)
            {
                shortest = distance;
                result = hits[i].collider.gameObject;
            }
        }

        return result;
    }

    public void TryAttack()
    {
        equipment.SendMessage("TryExecute");
    }

    public void Throw()
    {
        if (equipment == null)
            return;
        fxjt2d.connectedBody = null;
        fxjt2d.enabled = false;
        equipment.SendMessage("ThrowThisObj");
        equipment = null;
    }

    public void Drop()
    {
        if (equipment == null)
            return;
        fxjt2d.connectedBody = null;
        fxjt2d.enabled = false;
        equipment.SendMessage("Drop");
        equipment = null;
    }

    float GetDistance(Transform tf)
    {
        return (tf.position - transform.position).magnitude;
    }
}
