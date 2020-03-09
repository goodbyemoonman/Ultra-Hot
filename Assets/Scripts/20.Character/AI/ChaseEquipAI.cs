using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEquipAI : iAI
{
    SeekAlgorithm sa;
    GameObject target;
    List<Vector2Int> path;

    public ChaseEquipAI()
    {
        sa = new SeekAlgorithm();
        path = new List<Vector2Int>();
    }

    public void Check(GameObject who)
    {
        int layer = 1 << LayerMask.NameToLayer("EquipItem");
        if (who.GetComponent<EquipHolder>().IsEquipSomethig())
        {   //이미 무기를 착용 중
            //AI 변경해야됨.
        }

        if ((who.transform.position - target.transform.position).magnitude <= 0.5f)
        {   //무기에 접근.
            who.SendMessage("EKeyDown");
            //AI 변경해야됨.
        }

        if (path.Count == 0)
        {   //길이 없다면 생성
            path = sa.GetPath(V3Tov2I(who.transform.position), V3Tov2I(target.transform.position));
        }

        if ((who.transform.position - new Vector3(path[0].x, path[0].y)).magnitude < 0.1f)
        {   //해당 위치까지 이동
            path.Remove(path[0]);
        }

        List<GameObject> gos = CutOffOutOfSight(
            CheckBoundary(who, 5f, layer), who);
        SortByDistance(gos, who.transform.position);
        CutOffAlreadyEquiped(gos);

        if (gos.Count == 0)
            return;

        if(gos[0] != target)
        {
            target = gos[0];
            path = sa.GetPath(V3Tov2I(who.transform.position), V3Tov2I(target.transform.position));
        }

    }

    public void Do(GameObject who)
    {
        who.GetComponent<MoveHandler>().MoveToWorldPosition(path[0]);
    }

    Collider2D[] CheckBoundary(GameObject who, float radius, int layer)
    {
        return Physics2D.OverlapCircleAll(who.transform.position, radius, layer);
    }

    List<GameObject> CutOffOutOfSight(Collider2D[] cols, GameObject who)
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < cols.Length; i++)
        {
            if (IsBlockedWithWall(
                cols[i].transform.position,
                who.transform.position,
                1 << LayerMask.NameToLayer("Wall")) == false)
            {
                if (IsLookAt(who, cols[i].transform.position))
                {
                    result.Add(cols[i].gameObject);
                }

            }
        }

        return result;
    }

    void SortByDistance(List<GameObject> gos, Vector3 center)
    {
        GameObject tmp;
        int pivot;
        Stack<Vector2Int> lvrvs = new Stack<Vector2Int>();

        CheckPushNewLvRv(lvrvs, 0, gos.Count - 1, gos, center);
        while (lvrvs.Count > 0)
        {
            Vector2Int lvrv = lvrvs.Pop();
            pivot = lvrv.x;
            int lv = pivot + 1;
            int rv = lvrv.y;

            while (lv < rv)
            {
                //pivot보다 큰 값까지 lv를 우측으로 이동
                while (
                    IsRightBiggerThanLeft(
                        gos[lv].transform.position,
                        gos[pivot].transform.position, center)
                    &&
                    lv < rv
                    )
                {
                    lv++;
                }
                //pivot보다 작은 값까지 rv를 좌측으로 이동
                while (
                    IsRightBiggerThanLeft(
                        gos[pivot].transform.position,
                        gos[rv].transform.position, center)
                    &&
                    lv <= rv)
                {
                    rv--;
                }

                //
                if (lv < rv)
                {
                    tmp = gos[rv];
                    gos[rv] = gos[lv];
                    gos[lv] = tmp;
                }
            }
            tmp = gos[pivot];
            gos[pivot] = gos[rv];
            gos[rv] = tmp;

            CheckPushNewLvRv(lvrvs, lvrv.x, rv - 1, gos, center);
            CheckPushNewLvRv(lvrvs, lv, lvrv.y, gos, center);
        }
    }

    void CutOffAlreadyEquiped(List<GameObject> gos)
    {
        foreach(GameObject go in gos.ToArray())
        {
            if (go.transform.parent != null)
                gos.Remove(go);
        }
    }

    bool IsRightBiggerThanLeft(Vector3 lv, Vector3 rv, Vector3 center)
    {
        if ((lv - center).magnitude < (rv - center).magnitude)
            return true;
        else
            return false;
    }

    void CheckPushNewLvRv(Stack<Vector2Int> lvrvs, int lv, int rv, List<GameObject> gos, Vector3 center)
    {
        if (rv - lv + 1 > 2)
            lvrvs.Push(new Vector2Int(lv, rv));
        else if(rv - lv + 1 == 2)
        {
            if(IsRightBiggerThanLeft(
                gos[rv].transform.position,
                gos[lv].transform.position, center))
            {
                GameObject tmp = gos[rv];
                gos[rv] = gos[lv];
                gos[lv] = tmp;
            }
        }

    }

    bool IsBlockedWithWall(Vector3 v1, Vector3 v2, int layer)
    {
        RaycastHit2D hit = Physics2D.Linecast(v1, v2, layer);
        if (hit)
            return true;
        else
            return false;

    }

    bool IsLookAt(GameObject who, Vector3 target)
    {
        Vector3 pos = target - who.transform.position;
        Vector2 dir;

        float sin = Mathf.Sin(who.transform.eulerAngles.z * Mathf.Deg2Rad);
        float cos = Mathf.Cos(who.transform.eulerAngles.z * Mathf.Deg2Rad);

        dir.x = cos;
        dir.y = sin;

        float angle = Vector2.SignedAngle(dir, pos);

        if (Mathf.Cos(angle * Mathf.Deg2Rad) > 0)
            return true;
        else
            return false;

    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    Vector2Int V3Tov2I(Vector3 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }
}
