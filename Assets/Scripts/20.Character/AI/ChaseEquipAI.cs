using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEquipAI : iAI
{
    GameObject who;
    ActHandler actHandler;
    MoveHandler moveHandler;

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    List<Vector2> path;
    Vector3 targetPos;

    public ChaseEquipAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject who)
    {
        bca = b;
        sa = s;
        path = new List<Vector2>();
        this.who = who;
        actHandler = who.GetComponent<ActHandler>();
        moveHandler = who.GetComponent<MoveHandler>();
    }

    public void Check()
    {
        targetPos = bca.GetObjList()[0].transform.position;

        path = sa.GetPath((who.transform.position), targetPos);

        if (path.Count > 0 && 
            (who.transform.position - new Vector3(path[0].x, path[0].y)).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }
    }

    public void Do()
    {
        if (path.Count == 0)
            return;

        if ((who.transform.position - targetPos).magnitude <= 0.5f)
        {
            actHandler.InputEquip();
            return;
        }

        moveHandler.MoveToWorldPosition(path[0]);
    }
}
