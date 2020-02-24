using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkHandler : MonoBehaviour {
    AttackBase punch;
    EquipHolder holder;
    bool sw = true;

    private void Awake()
    {
        holder = GetComponent<EquipHolder>();
        punch = GetComponent<AttackBase>();
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

    public void LeftClick()
    {
        if (sw == false)
            return;
        if (holder.IsEquipSomethig())
            holder.TryAttack();
        else
            punch.TryExecute();
    }

    public void RightClick()
    {
        if (sw == false)
            return;
        if (!holder.IsEquipSomethig())
            return;

        holder.Throw();
    }
}
