using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static int WallLayer
    {
        get { return 1 << LayerMask.NameToLayer("Wall"); }
    }

    public static int EquipmentLayer
    {
        get { return 1 << LayerMask.NameToLayer("Equipment"); }
    }

    public static int PlayerLayer
    {
        get { return 1 << LayerMask.NameToLayer("PlayerCharacter"); }
    }

    public static int EnemyLayer
    {
        get { return 1 << LayerMask.NameToLayer("EnemyCharacter"); }
    }

    public static bool IsBlockWith(Vector2 v1, Vector2 v2, int layer)
    {
        RaycastHit2D hit = Physics2D.Linecast(v1, v2, layer);
        if (hit)
            return true;
        else
            return false;
    }
    
    public static bool IsLookAt(GameObject who, Vector3 target)
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

    public static Vector2Int V3ToV2I(Vector3 v)
    {
        Vector2Int[] vs = new Vector2Int[4];
        List<int> order = new List<int>();
        vs[0] = new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        vs[1] = new Vector2Int(Mathf.FloorToInt(v.x), Mathf.CeilToInt(v.y));
        vs[2] = new Vector2Int(Mathf.CeilToInt(v.x), Mathf.FloorToInt(v.y));
        vs[3] = new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

        order.Add(0);
        for(int i = 1; i < vs.Length; i++)
        {
            for(int j = 0; j < order.Count; j++)
            {
                if((vs[order[j]] - (Vector2)v).magnitude >= (vs[i] - (Vector2)v).magnitude)
                {
                    order.Insert(j, i);
                    break;
                }
            }
            if (i == order.Count)
            {
                order.Add(i);
            }
        }

        for (int i = 0; i < vs.Length; i++)
        {
            if (!IsWall(vs[order[i]]))
                return vs[order[i]];
        }

        return Vector2Int.zero;
    }

    public static bool IsWall(Vector2 crd)
    {
        Collider2D col = Physics2D.OverlapCircle(crd, 0.4f, Utility.WallLayer);
        if (col != null)
            return true;
        return
            false;
    }

    public static bool CanShoot(Vector2 shootPoint, Vector2 center, Vector2 target)
    {
        float cos = Mathf.Cos(45f * Mathf.Deg2Rad);
        float sin = Mathf.Sin(45f * Mathf.Deg2Rad);

        //8방향 검사
        for(int i = 0; i < 8; i++)
        {
            //원점이동
            shootPoint -= center;

            //회전
            shootPoint = new Vector2(shootPoint.x * cos - shootPoint.y * sin,
                shootPoint.x * sin + shootPoint.y * cos);

            //원래 위치로
            shootPoint += center;
            if (Physics2D.Linecast(shootPoint, target, WallLayer))
                return false;
        }

        return true;

    }
}
