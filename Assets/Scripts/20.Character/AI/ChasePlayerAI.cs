using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerAI : iAI
{
    GameObject who;
    ActHandler actHandler;
    MoveHandler moveHandler;

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    List<Vector2> path;
    Vector3 targetPos;

    public void Initialize()
    {
        path.Clear();
        targetPos = Vector3.zero;
    }

    public ChasePlayerAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject who)
    {
        bca = b;
        sa = s;
        path = new List<Vector2>();
        this.who = who;
        moveHandler = who.GetComponent<MoveHandler>();
        actHandler = who.GetComponent<ActHandler>();
    }

    bool checkAIChange(AIHolder aiHolder)
    {
        //바운더리에 오브젝트가 없네
        if (bca.GetObjList().Count == 0)
        {
            //뭔가 잘못됐어 플레이어는 꺼지지 않아
            if (bca.GetTarget().activeInHierarchy == false)
            {
                bca.SetTarget(null);
                aiHolder.SetAI(AIHolder.AIList.Patrol);
                return false;
            }
            //플레이어 추적 허가
            return true;
        }
        //바운더리에 오브젝트가 쫓던 오브젝트가 아닐 때
        else if (bca.GetTarget() != bca.GetObjList()[0])
        {
            //가까운 총 오브젝트 추격
            bca.SetTarget(bca.GetObjList()[0]);
            aiHolder.SetAI(AIHolder.AIList.ChaseEquip);
            return false;
        }

        return true;
    }

    public bool Check(AIHolder aiHolder)
    {
        if (checkAIChange(aiHolder) == false)
        {
            return false;
        }

        targetPos = bca.GetTarget().transform.position;

        path = sa.GetPath(who.transform.position, targetPos);

        if (path.Count > 0 &&
            (who.transform.position - new Vector3(path[0].x, path[0].y)).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }

        return true;
    }

    public void Do()
    {
        if (path.Count == 0)
            return;

        if ((who.transform.position - targetPos).magnitude <=
            actHandler.GetAtkRange()
            && Utility.CanShoot(
                who.transform.position + new Vector3(0.4f, 0f, 0f),
                who.transform.position, targetPos))
        {
            moveHandler.StopMove();
            float angle = Vector2.SignedAngle(Vector2.right, targetPos - who.transform.position);
            moveHandler.LookAt(angle);
            actHandler.InputEnemyDefaultAtk();
        }
        else
        {
            moveHandler.MoveToWorldPosition(path[0]);
        }
    }
}
