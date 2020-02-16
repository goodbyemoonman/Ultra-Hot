using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbAI : MonoBehaviour {
    MoveUnitCommand moveCommand;
    RotateUnitCommand rotateCommand;

    private void Awake()
    {
        moveCommand = new MoveUnitCommand(Vector2.right, Space.Self);
        rotateCommand = new RotateUnitCommand();
    }


}
