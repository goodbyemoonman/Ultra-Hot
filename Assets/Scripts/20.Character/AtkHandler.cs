using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkHandler : MonoBehaviour {
    public List<GameObject> punch;
    readonly float coolTime = 0.2f;
    bool isCooldown = false;
    public void Punch()
    {
        if (isCooldown)
            return;
        isCooldown = true;

        if (punch[0].activeInHierarchy)
            punch[1].SetActive(true);
        else
            punch[0].SetActive(true);

        Invoke("RefreshCooldown", coolTime);
    }

    void RefreshCooldown()
    {
        isCooldown = false;
    }
}
