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
        StartCoroutine(Patrol());
    }
    

    IEnumerator Patrol()
    {
        float _t = 0;
        float angle = 0;
        while (true)
        {
            moveCommand.Execute(gameObject);

            if (_t > 3)
            {
                angle += 180;
                rotateCommand.Initialize(angle);
                rotateCommand.Execute(gameObject);
                _t = 0;
            }
            _t += Time.deltaTime;
            yield return null;
        }
    }

}
