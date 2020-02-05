using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUnitCommand : ICommand {
    float angle;

    public void Initialize(float angle)
    {
        this.angle = angle;
    }

    public void Execute(GameObject obj)
    {
        obj.transform.eulerAngles =  new Vector3(0, 0, angle);
    }
}
