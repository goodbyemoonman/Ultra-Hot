using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActHandler : MonoBehaviour {
    EquipHolder holder;
    bool sw = true;

    private void Awake()
    {
        holder = GetComponent<EquipHolder>();
        GetComponent<HealthManager>().StateTeller += StateObserver;
    }

    private void OnEnable()
    {
        sw = true;
    }

    void StateObserver(HealthManager.CharacterState state)
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
