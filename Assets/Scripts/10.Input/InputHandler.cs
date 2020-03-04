using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TimeScaleManager tsm;
    public GameObject player;
    public bool canInput = true;
    Vector3 preMousePos;

    private void Update()
    {
        Vector2 _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 _mousePos = Input.mousePosition;
        TimeScaleInput(_mousePos, _direction);
        PlayerMoveInput(_mousePos, _direction);

        if (Input.GetMouseButtonDown(0))
            player.SendMessage("LeftClick");

        if (Input.GetMouseButtonDown(1))
            player.SendMessage("RightClick");

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.SendMessage("EKeyDown");
        }
    }

    void TimeScaleInput(Vector3 inputMousePos, Vector2 inputDir)
    {
        if (inputDir != Vector2.zero)
            tsm.SetInputType(INPUTTYPE.KEYBOARD);
        else if (preMousePos != inputMousePos)
            tsm.SetInputType(INPUTTYPE.MOUSE);
        else
            tsm.SetInputType(INPUTTYPE.NONE);

        preMousePos = Input.mousePosition;
    }

    void PlayerMoveInput(Vector3 inputMousePos, Vector2 inputDir)
    {
        if (canInput == false)
            return;

        player.SendMessage("MoveTo", inputDir);

        CommandToRotate(
            Vector2.SignedAngle(
                Vector2.right,
                Camera.main.ScreenToWorldPoint(inputMousePos) - player.transform.position));
    }

    void CommandToRotate(float angle)
    {
        player.SendMessage("LookAt", angle);
        SendMessage("RefreshSight");
    }

    float GetAngle(Vector2 objPos, Vector2 cursor)
    {
        return Vector2.SignedAngle(Vector2.right, cursor - objPos);
    }
}
