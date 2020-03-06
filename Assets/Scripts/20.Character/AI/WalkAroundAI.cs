using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAroundAI : AIBase
{
    Dir toMoveDir;
    float t;
    Vector2 originPos;

    public override void Initialize(GameObject who)
    {
        mh = who.GetComponent<MoveHandler>();
        toMoveDir = DetectLongDir(who);
        LookAt(toMoveDir, who);
        ToIntPos(who);
        t = 0;
        originPos = who.transform.position;
    }

    public override void Do(GameObject who)
    {
        MoveCommand(who);
        if ((originPos - (Vector2)who.transform.position).magnitude >= 1 || t > 1f)
        {
            ToIntPos(who);
            t = 0;
            toMoveDir = CheckLRF(who);
            LookAt(toMoveDir, who);
            originPos = who.transform.position;
            if(FindClosestEquipment(who) != null)
            {
                holder.Seek(FindClosestEquipment(who).transform);
            }
        }
        t += Time.deltaTime;
    }

    Dir DetectLongDir(GameObject who)
    {
        float distance;
        Dir result;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(
            who.transform.position, Vector2.down, 10f,
            1 << LayerMask.NameToLayer("Wall"));

        result = Dir.RIGHT;
        if (!hit)
        {
            distance = 10f;
        }
        else
        {
            distance = (hit.point - (Vector2)who.transform.position).magnitude;
        }

        hit = Physics2D.Raycast(
            who.transform.position, Vector2.up, 10f,
            1 << LayerMask.NameToLayer("Wall"));
        if (hit)
        {
            if (distance < (hit.point - (Vector2)who.transform.position).magnitude)
            {
                distance = (hit.point - (Vector2)who.transform.position).magnitude;
                result = Dir.LEFT;
            }
        }
        else
        {
            distance = 10f;
            result = Dir.LEFT;
        }

        hit = Physics2D.Raycast(
            who.transform.position, Vector2.right, 10f,
            1 << LayerMask.NameToLayer("Wall"));
        if (hit)
        {
            if (distance < (hit.point - (Vector2)who.transform.position).magnitude)
            {
                distance = (hit.point - (Vector2)who.transform.position).magnitude;
                result = Dir.FORWARD;
            }

        }
        else
        {
            distance = 10f;
            result = Dir.FORWARD;
        }

        hit = Physics2D.Raycast(
            who.transform.position, Vector2.left, 10f,
            1 << LayerMask.NameToLayer("Wall"));
        if (hit)
        {
            if (distance < (hit.point - (Vector2)who.transform.position).magnitude)
            {
                distance = (hit.point - (Vector2)who.transform.position).magnitude;
                result = Dir.BACKWARD;
            }
        }
        else
        {
            distance = 10f;
            result = Dir.BACKWARD;
        }

        return result;
    }

    void ToIntPos(GameObject who)
    {
        who.transform.position = new Vector3(
            Mathf.RoundToInt(who.transform.position.x),
            Mathf.RoundToInt(who.transform.position.y), 0);
    }

    Dir CheckLRF(GameObject who)
    {
        List<Dir> dirs = new List<Dir>();
        if (CanMoveTo(who, Dir.FORWARD))
        {
            dirs.Add(Dir.FORWARD);
            dirs.Add(Dir.FORWARD);
        }
        if (CanMoveTo(who, Dir.LEFT))
            dirs.Add(Dir.LEFT);
        if (CanMoveTo(who, Dir.RIGHT))
            dirs.Add(Dir.RIGHT);

        Dir result;

        if (dirs.Count != 0)
        {
            result = dirs[Random.Range(0, dirs.Count - 1)];
        }
        else
            result = Dir.BACKWARD;

        return result;
    }
}