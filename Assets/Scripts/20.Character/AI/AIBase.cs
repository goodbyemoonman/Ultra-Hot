using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase : MonoBehaviour {
    protected enum Dir { NONE, RIGHT, LEFT, FORWARD, BACKWARD }
    protected AIHolder holder;

    public abstract void Do(GameObject who);

    public IEnumerator RotateHelper(GameObject who, float angle)
    {
        float originAngle = who.transform.eulerAngles.z;
        for (float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            who.transform.eulerAngles =
                new Vector3(0, 0,
                Mathf.Lerp(originAngle, originAngle + angle, t * 4f));

            yield return null;
        }
        who.transform.eulerAngles = new Vector3(0, 0, originAngle + angle);
    }

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
}

