using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform tf;
    ChaseEquipAI chase;

    private void OnEnable()
    {
        chase = new ChaseEquipAI();
        chase.SetTarget(tf.gameObject);
    }

    private void Update()
    {
        chase.Check(gameObject);
        chase.Do(gameObject);
    }


    Collider2D[] SortByDistance(Collider2D[] objs, Vector3 center)
    {
        Collider2D tmp;
        int pivot;
        Stack<Vector2Int> lvrvs = new Stack<Vector2Int>();

        CheckAndPushNewLvRv(lvrvs, 0, objs.Length - 1, objs);

        while(lvrvs.Count != 0)
        {
            Vector2Int lvrv = lvrvs.Pop();
            pivot = lvrv.x;
            int lv = pivot + 1;
            int rv = lvrv.y;
            Debug.Log("p : " + pivot + ", Lv : " + lv + ", Rv : " + rv);
            while(lv < rv)
            {
                while (IsRBiggerThanL(
                    objs[lv].transform.position, 
                    objs[pivot].transform.position, center) && lv < rv)
                {
                    lv++;
                }
                while(IsRBiggerThanL(
                    objs[pivot].transform.position, 
                    objs[rv].transform.position, center) && lv <= rv)
                {
                    rv--;
                }
                
                if (lv < rv)
                {
                    tmp = objs[rv];
                    objs[rv] = objs[lv];
                    objs[lv] = tmp;
                    Debug.Log("Swap lv " + lv + ", rv " + rv);
                }
                else
                {
                    tmp = objs[pivot];
                    objs[pivot] = objs[rv];
                    objs[rv] = tmp;
                    Debug.Log("Swap Pivot " + pivot + ", rv " + rv);

                    CheckAndPushNewLvRv(lvrvs, lvrv.x, rv - 1, objs);
                    CheckAndPushNewLvRv(lvrvs, lv, lvrv.y, objs);
                }
            }
        }

        return objs;
    }

    bool IsRBiggerThanL(Vector3 lv, Vector3 rv, Vector3 center)
    {
        float lvdis = (lv - center).magnitude;
        float rvdis = (rv - center).magnitude;
        if (lvdis < rvdis)
            return true;
        return false;
    }

    void CheckAndPushNewLvRv(Stack<Vector2Int> lvrvs, int lv, int rv, Collider2D[] objs)
    {
        if(rv - lv + 1 > 2)
        {
            lvrvs.Push(new Vector2Int(lv, rv));
        }
        else if(rv - lv + 1 == 2)
        {
            if (IsRBiggerThanL(
                objs[rv].transform.position, 
                objs[lv].transform.position, transform.position)){
                Collider2D tmp = objs[lv];
                objs[lv] = objs[rv];
                objs[rv] = tmp;
            }
        }
    }

    Vector3 V2ItoV3(Vector2Int v)
    {
        return new Vector3(v.x, v.y);
    }
}

