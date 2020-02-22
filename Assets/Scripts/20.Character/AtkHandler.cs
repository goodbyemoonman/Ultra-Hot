using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkHandler : MonoBehaviour {
    AttackBase punch;
    EquipHolder holder;

    private void Awake()
    {
        holder = GetComponent<EquipHolder>();
        punch = GetComponent<AttackBase>();
    }

    public void LeftClick()
    {
        if (holder.IsEquipSomethig())
            holder.TryExecute();
        else
            punch.TryExecute();
    }

    public void RightClick()
    {
        if (!holder.IsEquipSomethig())
            return;

        holder.Throw();
    }
}
