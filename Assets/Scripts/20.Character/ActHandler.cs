using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActHandler : MonoBehaviour {
    EquipHolder holder;
    bool sw = false;

    private void Awake()
    {
        holder = GetComponent<EquipHolder>();
        GetComponent<HealthManager>().CharaStateTeller += CharaStateObserver;
    }

    private void OnEnable()
    {
        sw = true;
    }

    void CharaStateObserver(HealthManager.CharacterState state)
    {
        if (state == HealthManager.CharacterState.Sturn)
        {
            sw = false;
        }
        else
        {
            sw = true;
        }
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageStart:
                sw = true;
                break;
            default:
                sw = false;
                break;
        }
    }

    public void InputDefaultAtk()
    {
        if (sw == false)
            return;

        holder.TryAttack();
    }

    public void InputThrowAtk()
    {
        if (sw == false)
            return;

        holder.Throw();
    }

    public void InputEnemyDefaultAtk()
    {
        if (sw == false)
            return;

        holder.EnemyAtk();
    }

    public void InputEquip()
    {
        if (sw == false)
            return;

        holder.TryEquip();
    }

    public float GetAtkRange()
    {
        return holder.GetAtkRange();
    }

    public bool IsEquipSomething()
    {
        return holder.IsEquipSomethig();
    }
}
