using UnityEngine;

public class AIHolder : MonoBehaviour {
    public enum AIList { Patrol, ChasePlayer, ChaseEquip }

    [SerializeField]
    BoundaryCheckAlgorithm bca;
    [SerializeField]
    SeekAlgorithm sa;
    ChasePlayerAI chasePlayerAI;
    ChaseEquipAI chaseEquipAI;
    PatrolAI patrolAI;
    AbstractAI aiNow;

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

    private void OnEnable()
    {
        sa.Initialize();
        bca.Initialize();
        patrolAI.Initialize();
        chasePlayerAI.Initialize();
        chaseEquipAI.Initialize();
        SetAI(AIList.Patrol);
    }

    public void SetAI(AIList ai)
    {
        //Debug.Log("switch AI to " + ai);
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
        BoundaryCheck();
        if(aiNow.Check(this))
            aiNow.Do();
    }
    
    void BoundaryCheck()
    {
        int targetLayer;
        if (actHandler.IsEquipSomething())
            targetLayer = Utility.PlayerLayer;
        else
            targetLayer = Utility.PlayerLayer | Utility.EquipmentLayer;

        if (bca.CheckObjListInSight(gameObject, 5f, targetLayer))
        {
            while (sa.GetPath(transform.position, bca.GetObjList()[0].transform.position).Count == 0)
            {
                bca.GetObjList().RemoveAt(0);
            }
        }
    }
}
