using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TimeScaleManager tsm;
    MoveUnitCommand moveUnitCommand = new MoveUnitCommand(Vector2.zero);
    RotateUnitCommand rotateUnitCommand = new RotateUnitCommand();
    public GameObject player;
    public bool canMove = true;
    Vector3 preMousePos;

    private void Start()
    {
        StartCoroutine(InputWatcher());
    }

    IEnumerator InputWatcher()
    {
        float _timeScale = 0;

        while (canMove)
        {
            if (preMousePos != Input.mousePosition)
                _timeScale = 0.2f;
            else
                _timeScale = 0f;
            preMousePos = Input.mousePosition;
            CommandToRotate(GetAngle(player.transform.position,
            Camera.main.ScreenToWorldPoint(preMousePos)));

            if (IsMove())
            {
                Vector2 direction = new Vector2(Input.GetAxis("Horizontal"),
                    Input.GetAxis("Vertical"));
                CommandToMove(direction.normalized);
                _timeScale = 1f;
            }
            
            tsm.SetTimeScale(_timeScale);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    void CommandToMove(Vector2 dir)
    {
        moveUnitCommand.SetDirection(dir);
        moveUnitCommand.Execute(player);
    }

    void CommandToRotate(float angle)
    {
        rotateUnitCommand.Initialize(angle);
        rotateUnitCommand.Execute(player);
    }

    bool IsInputEmpty()
    {
        if (Input.GetKey(KeyCode.W))
            return false;
        if (Input.GetKey(KeyCode.A))
            return false;
        if (Input.GetKey(KeyCode.S))
            return false;
        if (Input.GetKey(KeyCode.D))
            return false;

        if (Input.mousePosition != preMousePos)
            return false;

        return true;
    }

    bool IsMove()
    {
        return Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) != 0;
    }

    float GetAngle(Vector2 objPos, Vector2 cursor)
    {
        cursor -= objPos;
        cursor.Normalize();

        return Mathf.Atan2(cursor.y, cursor.x) * Mathf.Rad2Deg;
    }
}
