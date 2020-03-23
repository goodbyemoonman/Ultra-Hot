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
    bool canInputKey = false;
    Vector3 preMousePos;
    bool isActed;

    private void Awake()
    {
        mh = player.GetComponent<MoveHandler>();
        ah = player.GetComponent<ActHandler>();
        sm = GetComponent<SightManager>();
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    private void Update()
    { 
        if (!canInputKey)
            return;
        Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 mousePos = Input.mousePosition;
        ToTimeScale(mousePos, dir);
        ToPlayer(mousePos, dir);
        isActed = false;
        if (Input.GetMouseButtonDown(0))
        {
            isActed = true;
            ah.InputDefaultAtk();
        }

        if (Input.GetMouseButtonDown(1))
        {
            isActed = true;
            ah.InputThrowAtk();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isActed = true;
            ah.InputEquip();
        }
        if (isActed)
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
    
    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageReady:
                player.transform.position = Vector3.zero;
                break;
            case GameStateList.StageStart:
                canInputKey = true;
                break;
            case GameStateList.Win:
            case GameStateList.Defeat:
                canInputKey = false;
                mh.StopMove();
                break;
        }
    }
}
