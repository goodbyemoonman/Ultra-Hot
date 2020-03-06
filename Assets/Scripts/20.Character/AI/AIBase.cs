using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase {
    protected enum Dir { NONE, RIGHT, LEFT, FORWARD, BACKWARD }
    protected AIHolder holder;
    protected MoveHandler mh;

    public abstract void Initialize(GameObject who);

    public abstract void Do(GameObject who);

    protected bool CanMoveTo(GameObject who, Dir dir)
    {
        Vector2 direction;
        switch (dir)
        {
            case Dir.FORWARD:
                direction = Vector2.right;
                break;
            case Dir.LEFT:
                direction = Vector2.up;
                break;
            case Dir.RIGHT:
                direction = Vector2.down;
                break;
            default:
                direction = Vector2.zero;
                break;
        }

        RaycastHit2D hit;
        float angle = who.transform.eulerAngles.z;
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector2 checkDir = new Vector2(cos * direction.x - sin * direction.y,
            sin * direction.x + cos * direction.y);
        hit = Physics2D.Raycast(who.transform.position, checkDir, 1f,
            1 << LayerMask.NameToLayer("Wall"));
        Debug.DrawRay(who.transform.position, checkDir, Color.blue, 1f);
        if (!hit)
            return true;
        else
            return false;

    }

    public void SetHoler(AIHolder holder)
    {
        this.holder = holder;
    }

    protected void LookAt(Dir direction, GameObject who)
    {
        float angle = 0f;
        switch (direction)
        {
            case Dir.BACKWARD:
                angle = -180f;
                break;
            case Dir.RIGHT:
                angle = -90f;
                break;
            case Dir.FORWARD:
                angle = 0f;
                break;
            case Dir.LEFT:
                angle = 90f;
                break;
            default:
                angle = 0f;
                break;
        }

        if (mh == null)
            mh = who.GetComponent<MoveHandler>();
        mh.LookAt(who.transform.eulerAngles.z + angle);
    }

    protected void MoveCommand(GameObject who)
    {
        if (mh == null)
            mh = who.GetComponent<MoveHandler>();
        mh.MoveForward();
    }

    protected GameObject FindClosestEquipment(GameObject who)
    {
        float boundary = 5f;
        int targetLayer = 1 << LayerMask.NameToLayer("EquipItem");
        int wallLayer = 1 << LayerMask.NameToLayer("Wall");
        Collider2D[] hits = CheckBoundary(who, boundary, targetLayer);
        GameObject result = null;
        float sd = 5f;

        for (int i = 0; i < hits.Length; i++)
        {
            float distance = GetDistance(hits[i].gameObject, who);
            
            Debug.DrawRay(
                hits[i].transform.position,
                (who.transform.position - hits[i].transform.position).normalized,
                Color.red, 1f);

            if (!IsBlockedWithWall(who, hits[i].gameObject))
            {
                if (sd > distance)
                {
                    sd = distance;
                    result = hits[i].gameObject;
                }
            }
        }

        return result;
    }

    protected GameObject FindClosestObjInSight(GameObject who)
    {
        Collider2D[] laps = CheckBoundary(who, 8f, 
            1 << LayerMask.NameToLayer("PlayerCharacter") | 
            1 << LayerMask.NameToLayer("EquipItem"));

        GameObject result = null;
        float shortDistance = 8f;

        for(int i= 0; i < laps.Length; i++)
        {
            if(!IsBlockedWithWall(who, laps[i].gameObject)&&
                IsLookAt(who, laps[i].gameObject)){
                float d = GetDistance(who, laps[i].gameObject);
                if (d < shortDistance)
                {
                    result = laps[i].gameObject;
                    shortDistance = d;
                }
            }
        }

        return result;
    }

    protected bool IsBlockedWithWall(GameObject go1, GameObject go2)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            go1.transform.position,
            (go2.transform.position - go1.transform.position).normalized,
            GetDistance(go1, go2),
            1 << LayerMask.NameToLayer("Wall"));

        if (hit)
            return true;
        else
            return false;
    }
    
    protected Collider2D[] CheckBoundary(GameObject who, float radius, int targetLayer)
    {
        Collider2D[] result = Physics2D.OverlapCircleAll(
            who.transform.position, radius, targetLayer);
        return result;
    }

    protected float GetDistance(GameObject go1, GameObject go2)
    {
        return (go1.transform.position - go2.transform.position).magnitude;
    }

    protected bool IsLookAt(GameObject who, GameObject target)
    {
        Vector3 pos = target.transform.position - who.transform.position;
        Vector2 dir = Vector2.right;
        float sin = Mathf.Sin(who.transform.eulerAngles.z * Mathf.Deg2Rad);
        float cos = Mathf.Cos(who.transform.eulerAngles.z * Mathf.Deg2Rad);
        dir.x = Vector2.right.x * cos - Vector2.right.y * sin;
        dir.y = Vector2.right.x * sin + Vector2.right.y * cos;

        float angle = Vector2.SignedAngle(dir, (Vector2)pos);
        Debug.Log("angle = " + angle.ToString());
        if (Mathf.Cos(angle * Mathf.Deg2Rad) > 0)
            return true;
        else
            return false;
    }
}

