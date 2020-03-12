using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerAI : iAI
{
    GameObject who;
    AIHolder aiHolder;
    ActHandler atkHandler;
    MoveHandler moveHandler;

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    List<Vector2> path;
    GameObject target;

    public ChasePlayerAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject who)
    {
        bca = b;
        sa = s;
        path = new List<Vector2>();
        this.who = who;
        aiHolder = who.GetComponent<AIHolder>();
        moveHandler = who.GetComponent<MoveHandler>();
        atkHandler = who.GetComponent<ActHandler>();
    }

    public bool Check()
    {
        int targetLayer;
        if (atkHandler.IsEquipSomething())
            targetLayer = Utility.PlayerLayer;
        else
            targetLayer = Utility.PlayerLayer | Utility.EquipmentLayer;
               
        List<GameObject> gos = bca.GetObjListInSight(who, 8f, targetLayer);

        if (gos.Count == 0)
        {
            aiHolder.SetAI(AIHolder.AIList.Patrol);
            return false;
        }
        else if (gos[0].CompareTag("Equipment"))
        {
            aiHolder.SetAI(AIHolder.AIList.ChaseEquip);
            return false;
        }
        else
            target = gos[0];

        path = sa.GetPath(
            (who.transform.position),
            (target.transform.position));

        if (path.Count > 0 &&
            (who.transform.position - new Vector3(path[0].x, path[0].y)).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }
        

        if ((who.transform.position - target.transform.position).magnitude <=
            atkHandler.GetAtkRange()
            && Utility.CanShoot(
                who.transform.position + new Vector3(0.4f, 0f, 0f),
                who.transform.position, 
                target.transform.position))
        {
            path.Clear();
            return true;
        }

        if (path.Count == 0)
        {
            aiHolder.SetAI(AIHolder.AIList.Patrol);
            return false;
        }

        return true;
    }

    public void Do()
    {
        if (path.Count > 0)
            moveHandler.MoveToWorldPosition(path[0]);
        else
        {
            moveHandler.StopMove();
            float angle = Vector2.SignedAngle(Vector2.right, target.transform.position - who.transform.position);
            moveHandler.LookAt(angle);
            atkHandler.InputEnemyDefaultAtk();
        }
    }
}
