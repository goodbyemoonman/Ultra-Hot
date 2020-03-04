using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUnitCommand : MonoBehaviour {

    public void LookAt(float angle)
    {
        gameObject.transform.eulerAngles =  new Vector3(0, 0, angle);
    }
}
