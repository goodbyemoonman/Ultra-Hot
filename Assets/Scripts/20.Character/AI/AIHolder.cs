using UnityEngine;

public class AIHolder : MonoBehaviour {
    public enum AIList { Patrol, ChasePlayer, ChaseEquip }

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    ChasePlayerAI chasePlayerAI;
    ChaseEquipAI chaseEquipAI;
    PatrolAI patrolAI;
    iAI aiNow;

    ActHandler actHandler;

    private void Awake()
    {
        actHandler = GetComponent<ActHandler>();
        bca = new BoundaryCheckAlgorithm();
        sa = new SeekAlgorithm();
        chasePlayerAI = new ChasePlayerAI(bca, sa, gameObject);
        chaseEquipAI = new ChaseEquipAI(bca, sa, gameObject);
        patrolAI = new PatrolAI(bca, sa, gameObject);
        aiNow = patrolAI;
    }

    public void SetAI(AIList ai)
    {
        Debug.Log("switch AI to " + ai);
        switch (ai)
        {
            case AIList.Patrol:
                aiNow = patrolAI;
                break;
            case AIList.ChaseEquip:
                aiNow = chaseEquipAI;
                break;
            case AIList.ChasePlayer:
                aiNow = chasePlayerAI;
                break;
        }
    }

    private void Update()
    {
        ChooseAI();
        aiNow.Check();
        aiNow.Do();
    }

    void ChooseAI()
    {
        int targetLayer;
        if (actHandler.IsEquipSomething())
            targetLayer = Utility.PlayerLayer;
        else
            targetLayer = Utility.PlayerLayer | Utility.EquipmentLayer;

        if (bca.CheckObjListInSight(gameObject, 8f, targetLayer))
        {
            if (bca.GetObjList()[0].CompareTag("Player"))
            {
                //플레이어 추적으로 변환.
                aiNow = chasePlayerAI;
            }
            else
            {
                //무기 추적으로 변환.
                aiNow = chaseEquipAI;
            }
        }
        else
        {
            //패트롤로 변환
            aiNow = patrolAI;
        }

    }
}
