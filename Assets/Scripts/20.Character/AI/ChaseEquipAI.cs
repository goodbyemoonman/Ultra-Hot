using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEquipAI : AbstractAI
{
    public void Initialize()
    {
        path.Clear();
        targetPos = Vector3.zero;
    }

    public ChaseEquipAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject who)
    {
        bca = b;
        sa = s;
        path = new List<Vector2>();
        this.who = who;
        actHandler = who.GetComponent<ActHandler>();
        moveHandler = who.GetComponent<MoveHandler>();
    }

    public override bool CheckAIChange(AIHolder aiHolder)
    {
        //타겟이 없어졌네?
        if(bca.GetTarget().transform.parent != null || 
            bca.GetTarget().activeInHierarchy == false)
        {
            bca.SetTarget(null);
            aiHolder.SetAI(AIHolder.AIList.Patrol);
            return false;
        }

        //바운더리에 오브젝트가 없네 (길찾기로 가던 도중 타겟에서 멀어질 수 있어)
        if (bca.GetObjList().Count == 0)
        {
            //그래도 타겟은 있으니까
            return true;
        }
        //바운더리의 오브젝트와 쫓던 오브젝트가 다를 때
        else if (bca.GetTarget() != bca.GetObjList()[0])
        {
            //새 오브젝트를 추적
            bca.SetTarget(bca.GetObjList()[0]);
            if (bca.GetObjList()[0].CompareTag("Player"))
                aiHolder.SetAI(AIHolder.AIList.ChasePlayer);
            else
                aiHolder.SetAI(AIHolder.AIList.ChaseEquip);
            return false;
        }

        return true;
    }

    public override bool Check(AIHolder aiHolder)
    {
        if (CheckAIChange(aiHolder) == false)
        {   //ai 변경
            return false;
        }

        targetPos = bca.GetTarget().transform.position;

        path = sa.GetPath((who.transform.position), targetPos);

        if (path.Count > 0 && 
            (who.transform.position - new Vector3(path[0].x, path[0].y)).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }

        return true;
    }

    public override void Do()
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
