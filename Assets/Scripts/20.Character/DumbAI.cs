using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbAI : MonoBehaviour {
    MoveUnitCommand moveCommand;
    RotateUnitCommand rotateCommand;

    private void Awake()
    {
        SendMessage("EKeyDown");
    }


}
