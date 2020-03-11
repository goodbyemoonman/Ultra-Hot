using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerAI : iAI
{
    GameObject who;
    AIHolder ah;
    EquipHolder eh;
    MoveHandler mh;

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
        ah = who.GetComponent<AIHolder>();
        eh = who.GetComponent<EquipHolder>();
        mh = who.GetComponent<MoveHandler>();
    }

    public bool Check()
    {
        int targetLayer;
        if (eh.IsEquipSomethig())
            targetLayer = Utility.PlayerLayer;
        else
            targetLayer = Utility.PlayerLayer | Utility.EquipmentLayer;
               
        List<GameObject> gos = bca.GetObjListInSight(who, 8f, targetLayer);

        if (gos.Count == 0)
        {
            ah.SetAI(AIHolder.AIList.WalkAround);
            return false;
        }
        else if (gos[0].CompareTag("Equipment"))
        {
            ah.SetAI(AIHolder.AIList.ChaseEquip);
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
            eh.GetAtkRange()
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
            ah.SetAI(AIHolder.AIList.WalkAround);
            return false;
        }

        return true;
    }

    public void Do()
    {
        if (path.Count > 0)
            mh.MoveToWorldPosition(path[0]);
        else
        {
            mh.StopMove();
            float angle = Vector2.SignedAngle(Vector2.right, target.transform.position - who.transform.position);
            mh.LookAt(angle);
            eh.EnemyAtk();
        }
    }
}
