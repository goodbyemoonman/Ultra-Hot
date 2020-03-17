using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerAI : iAI
{
    GameObject who;
    ActHandler atkHandler;
    MoveHandler moveHandler;

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    List<Vector2> path;
    Vector3 target;

    public ChasePlayerAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject who)
    {
        bca = b;
        sa = s;
        path = new List<Vector2>();
        this.who = who;
        moveHandler = who.GetComponent<MoveHandler>();
        atkHandler = who.GetComponent<ActHandler>();
    }

    public void Check()
    {
        target = bca.GetObjList()[0].transform.position;

        path = sa.GetPath(who.transform.position, target);

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

        if ((who.transform.position - target).magnitude <=
            atkHandler.GetAtkRange()
            && Utility.CanShoot(
                who.transform.position + new Vector3(0.4f, 0f, 0f),
                who.transform.position, target))
        {
            moveHandler.StopMove();
            float angle = Vector2.SignedAngle(Vector2.right, target - who.transform.position);
            moveHandler.LookAt(angle);
            atkHandler.InputEnemyDefaultAtk();
        }
        else
        {
            moveHandler.MoveToWorldPosition(path[0]);
        }
    }
}
