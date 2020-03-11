using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAroundAI : iAI
{
    enum DirList { None,
        Right, Left, Forward, Backward }
    enum WorldDirList { None,
        Down, Up, Right, Left }
    readonly Vector2Int[] dirs = {Vector2Int.zero,
        Vector2Int.down, Vector2Int.up, Vector2Int.right, Vector2Int.left };
    readonly float[] angles = { 0f,
        -90f, 90f, 0f, 180f };
    GameObject who;
    AIHolder ah;
    EquipHolder eh;
    MoveHandler mh;
    WorldDirList dirNow;
    bool sw;

    BoundaryCheckAlgorithm bca;

    public WalkAroundAI(BoundaryCheckAlgorithm b, GameObject w)
    {
        who = w;
        ah = who.GetComponent<AIHolder>();
        eh = who.GetComponent<EquipHolder>();
        mh = who.GetComponent<MoveHandler>();

        bca = b;
        dirNow = WorldDirList.None;
        sw = true;
    }
    
    WorldDirList DetectLongDir()
    {
        float distance;
        WorldDirList result;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(
            who.transform.position, dirs[(int)WorldDirList.Right], 10f,
            Utility.WallLayer);

        result = WorldDirList.Right;
        if (!hit)
            distance = 10f;
        else
            distance = (hit.point - (Vector2)who.transform.position).magnitude;

        hit = Physics2D.Raycast(
            who.transform.position, dirs[(int)WorldDirList.Left], 10f,
            Utility.WallLayer);
        if (hit)
        {
            if (distance < (hit.point - (Vector2)who.transform.position).magnitude)
            {
                distance = (hit.point - (Vector2)who.transform.position).magnitude;
                result = WorldDirList.Left;
            }
        }
        else
        {
            distance = 10f;
            result = WorldDirList.Left;
        }

        hit = Physics2D.Raycast(
            who.transform.position, dirs[(int)WorldDirList.Up], 10f,
            Utility.WallLayer);
        if (hit)
        {
            if (distance < (hit.point - (Vector2)who.transform.position).magnitude)
            {
                distance = (hit.point - (Vector2)who.transform.position).magnitude;
                result = WorldDirList.Up;
            }
        }
        else
        {
            distance = 10f;
            result = WorldDirList.Up;
        }

        hit = Physics2D.Raycast(
            who.transform.position, dirs[(int)WorldDirList.Down], 10f,
            Utility.WallLayer);
        if (hit)
        {
            if (distance < (hit.point - (Vector2)who.transform.position).magnitude)
            {
                distance = (hit.point - (Vector2)who.transform.position).magnitude;
                result = WorldDirList.Down;
            }
        }
        else
        {
            distance = 10f;
            result = WorldDirList.Down;
        }

        return result;
    }

    DirList CheckNextSelfDirection()
    {
        List<DirList> results = new List<DirList>();
        if (CanMoveToSlefDirection(who, dirs[(int)DirList.Forward]))
        {
            results.Add(DirList.Forward);
            results.Add(DirList.Forward);
        }
        if (CanMoveToSlefDirection(who, dirs[(int)DirList.Left]))
            results.Add(DirList.Left);
        if (CanMoveToSlefDirection(who, dirs[(int)DirList.Right]))
            results.Add(DirList.Right);

        DirList result;

        if (results.Count != 0)
        {
            result = results[Random.Range(0, results.Count - 1)];
        }
        else
            result = DirList.Backward;

        return result;
    }
   
    bool CanMoveToSlefDirection(GameObject who, Vector2 dir)
    {
        float angle = who.transform.eulerAngles.z * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        Vector2 checkDir = new Vector2(cos * dir.x - sin * dir.y, sin * dir.x + cos * dir.y);
        RaycastHit2D hit = Physics2D.Linecast(who.transform.position, 
            (Vector2)who.transform.position + checkDir, Utility.WallLayer);
        Debug.DrawLine(who.transform.position, 
            (Vector2)who.transform.position + checkDir, Color.blue, 1f);
        if (!hit)
            return true;
        else
            return false;
    }

    public bool Check()
    {
        if(dirNow == WorldDirList.None)
        {
            dirNow = DetectLongDir();
        }
        int targetLayer;
        if (eh.IsEquipSomethig())
            targetLayer = Utility.PlayerLayer;
        else
            targetLayer = Utility.PlayerLayer | Utility.EquipmentLayer;

        List<GameObject> objs = bca.GetObjListInSight(who, 8f, targetLayer);

        if(objs.Count != 0)
        {
            if (objs[0].CompareTag("Player"))
                ah.SetAI(AIHolder.AIList.ChasePlayer);
            else
                ah.SetAI(AIHolder.AIList.ChaseEquip);
            return false;
        }

        //round된 좌표와 현재 좌표의 차이가 0.1 이하일 때 방향검사하여 방향 바꿔줌.
        Vector2 v = Utility.V3ToV2I(who.transform.position);
        if(((Vector2)who.transform.position - v).magnitude < 0.1f && sw)
        {
            DirList dir = CheckNextSelfDirection();
            dirNow = GetWorldDir(dirNow, dir);
            sw = false;
        }
        else if(((Vector2)who.transform.position - v).magnitude > 0.5f)
            sw = true;

        return true;
    }

    WorldDirList GetWorldDir(WorldDirList origin, DirList rotate)
    {
        if (rotate == DirList.Forward)
            return origin;

        if (rotate == DirList.Backward)
        {
            switch (origin)
            {
                case WorldDirList.Down:
                    return WorldDirList.Up;
                case WorldDirList.Up:
                    return WorldDirList.Down;
                case WorldDirList.Right:
                    return WorldDirList.Left;
                case WorldDirList.Left:
                    return WorldDirList.Right;
            }
        }

        if(rotate == DirList.Left)
        {
            switch (origin)
            {
                case WorldDirList.Down:
                    return WorldDirList.Right;
                case WorldDirList.Right:
                    return WorldDirList.Up;
                case WorldDirList.Up:
                    return WorldDirList.Left;
                case WorldDirList.Left:
                    return WorldDirList.Down;
            }
        }

        if(rotate == DirList.Right)
        {
            switch (origin)
            {
                case WorldDirList.Up:
                    return WorldDirList.Right;
                case WorldDirList.Right:
                    return WorldDirList.Down;
                case WorldDirList.Down:
                    return WorldDirList.Left;
                case WorldDirList.Left:
                    return WorldDirList.Up;
            }
        }

        return WorldDirList.None;
    }

    public void Do()
    {
        mh.LookAt(angles[(int)dirNow]);
        mh.MoveToWorldDirection(dirs[(int)dirNow]);
    }
}