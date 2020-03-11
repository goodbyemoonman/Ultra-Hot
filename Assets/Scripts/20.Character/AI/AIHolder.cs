using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHolder : MonoBehaviour {
    public enum AIList { WalkAround, ChasePlayer, ChaseEquip }

    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    ChasePlayerAI cpAI;
    ChaseEquipAI ceAI;
    WalkAroundAI waAI;

    iAI aiNow;

    private void Awake()
    {
        bca = new BoundaryCheckAlgorithm();
        sa = new SeekAlgorithm();
        cpAI = new ChasePlayerAI(bca, sa, gameObject);
        ceAI = new ChaseEquipAI(bca, sa, gameObject);
        waAI = new WalkAroundAI(bca, gameObject);
        aiNow = waAI;
    }

    public void SetAI(AIList ai)
    {
        Debug.Log("switch AI to " + ai);
        switch (ai)
        {
            case AIList.WalkAround:
                aiNow = waAI;
                break;
            case AIList.ChaseEquip:
                aiNow = ceAI;
                break;
            case AIList.ChasePlayer:
                aiNow = cpAI;
                break;
        }
    }

    private void Update()
    {
        if (aiNow.Check())
            aiNow.Do();
    }
}
