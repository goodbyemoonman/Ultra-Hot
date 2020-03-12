using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TimeScaleManager tsm;
    public GameObject player;
    SightManager sm;
    MoveHandler mh;
    ActHandler ah;
    public bool canInput = true;
    Vector3 preMousePos;
    bool isAct;

    private void Awake()
    {
        mh = player.GetComponent<MoveHandler>();
        ah = player.GetComponent<ActHandler>();
        sm = GetComponent<SightManager>();
    }

    private void Update()
    {
        Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 mousePos = Input.mousePosition;
        ToTimeScale(mousePos, dir);
        ToPlayer(mousePos, dir);
        isAct = false;
        if (Input.GetMouseButtonDown(0))
        {
            isAct = true;
            ah.InputDefaultAtk();
        }

        if (Input.GetMouseButtonDown(1))
        {
            isAct = true;
            ah.InputThrowAtk();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isAct = true;
            ah.InputEquip();
        }
        if (isAct)
            tsm.ActTimeScale();
    }

    void ToTimeScale(Vector3 inputMousePos, Vector2 inputDir)
    {
        if (inputDir != Vector2.zero)
            tsm.SetInputType(INPUTTYPE.KEYBOARD);
        else if (preMousePos != inputMousePos)
            tsm.SetInputType(INPUTTYPE.MOUSE);
        else
            tsm.SetInputType(INPUTTYPE.NONE);

        preMousePos = Input.mousePosition;
    }

    void ToPlayer(Vector3 inputMousePos, Vector2 inputDir)
    {
        if (canInput == false)
            return;

        mh.MoveToWorldDirection(inputDir.normalized);
        float angle = Vector2.SignedAngle(
                Vector2.right,
                Camera.main.ScreenToWorldPoint(inputMousePos) - player.transform.position);

        CommandToRotate(angle);
    }

    void CommandToRotate(float angle)
    {
        mh.LookAt(angle);
        sm.RefreshSight();
    }

    float GetAngle(Vector2 objPos, Vector2 cursor)
    {
        return Vector2.SignedAngle(Vector2.right, cursor - objPos);
    }
}
