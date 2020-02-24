using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : HealthManager {
    public override void GetDamaged(int damage)
    {
        Debug.Log("패배");
    }
}
