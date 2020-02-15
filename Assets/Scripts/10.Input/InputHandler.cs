using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TimeScaleManager tsm;
    MoveUnitCommand moveUnitCommand = new MoveUnitCommand(Vector2.zero);
    RotateUnitCommand rotateUnitCommand = new RotateUnitCommand();
    Vector2 velocity = Vector2.zero;
    public GameObject player;
    public bool canMove = true;

    private void Awake()
    {
        GetComponent<GameManager>().GameStateObserver += GameStateObserve;
    }

    private void GameStateObserve(GAMESTATE state)
    {
        if (state == GAMESTATE.MAP_READY)
        {
            StartCoroutine(ToTimeScale());
            StartCoroutine(ToPlayerMove());
        }
    }

    private void Update()
    {
        moveUnitCommand.Execute(player);
    }

    IEnumerator ToTimeScale()
    {
        Vector3 _preMousePos = Vector3.zero;
        Vector2 direction = Vector2.zero;

        while (canMove)
        {
            direction = new Vector2(Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical"));

            if (direction != Vector2.zero)
                tsm.SetInputType(INPUTTYPE.KEYBOARD);
            else if (_preMousePos != Input.mousePosition)
                tsm.SetInputType(INPUTTYPE.MOUSE);
            else
                tsm.SetInputType(INPUTTYPE.NONE);

            _preMousePos = Input.mousePosition;

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator ToPlayerMove()
    {
        while (canMove)
        {
            Vector2 direction = new Vector2(Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical"));

            moveUnitCommand.SetDirection(direction);

            CommandToRotate(GetAngle(player.transform.position,
            Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    void CommandToRotate(float angle)
    {
        rotateUnitCommand.Initialize(angle);
        rotateUnitCommand.Execute(player);
    }

    float GetAngle(Vector2 objPos, Vector2 cursor)
    {
        cursor -= objPos;
        cursor.Normalize();

        return Mathf.Atan2(cursor.y, cursor.x) * Mathf.Rad2Deg;
    }
}
