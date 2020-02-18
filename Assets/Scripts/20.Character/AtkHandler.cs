using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkHandler : MonoBehaviour {
    AttackBase punch;
    public GameObject equipment;

    private void Awake()
    {
        punch = GetComponent<AttackBase>();
    }

    public void LeftClick()
    {
        if (equipment == null)
            punch.TryExecute();
        else
            equipment.SendMessage("TryExecute");
    }

    public void RightClick()
    {
        if (equipment == null)
            return;

        equipment.SendMessage("ThrowThisObj");
        equipment = null;
    }
}
