using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEquipAI : iAI
{
    GameObject who;
    AIHolder ah;
    EquipHolder eh;
    MoveHandler mh;

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    List<Vector2> path;
    GameObject target;

    public ChaseEquipAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject who)
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
        if (eh.IsEquipSomethig())
        {   //이미 무기를 착용 중
            ah.SetAI(AIHolder.AIList.WalkAround);
            return false;
        }

        //새로운 타겟 Equipment 탐색.
        List<GameObject> gos = bca.GetObjListInSight(who, 8f, Utility.EquipmentLayer | Utility.PlayerLayer);
        if (gos.Count == 0)
        {
            ah.SetAI(AIHolder.AIList.WalkAround);
            return false;
        }
        else if (gos[0].layer == Utility.PlayerLayer)
        {
            ah.SetAI(AIHolder.AIList.ChasePlayer);
            return false;
        }
        else if (gos[0] != target)
        {
            target = gos[0];
        }

        //타겟 Euipqment 에 접근?
        if ((who.transform.position - target.transform.position).magnitude <= 0.5f)
        {   
            eh.EKeyDown();
            ah.SetAI(AIHolder.AIList.WalkAround);
            return false;
        }

        path = sa.GetPath(
            (who.transform.position),
            (target.transform.position));

        if (path.Count > 0 && 
            (who.transform.position - new Vector3(path[0].x, path[0].y)).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }

        //맨처음에 현재 위치 넣는것때문에 그런듯.. 반올림을 찾아가버리노..

        if (path.Count == 0)
        {
            ah.SetAI(AIHolder.AIList.WalkAround);
            return false;
        }

        return true;
    }

    public void Do()
    {
       mh.MoveToWorldPosition(path[0]);
    }
}
