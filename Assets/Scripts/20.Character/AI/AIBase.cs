using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase {
    protected enum Dir { NONE, RIGHT, LEFT, FORWARD, BACKWARD }
    protected AIHolder holder;
    RotateUnitCommand rotateCommand;
    MoveUnitCommand moveCommand;

    public void Awake()
    {
        rotateCommand = new RotateUnitCommand();
        moveCommand = new MoveUnitCommand(Vector2.right * 0.5f, Space.Self);
    }

    public abstract void Initialize(GameObject who);

    public abstract void Do(GameObject who);

    protected bool CanMoveTo(GameObject who, Dir dir)
    {
        Vector2 direction;
        switch (dir)
        {
            case Dir.FORWARD:
                direction = Vector2.right;
                break;
            case Dir.LEFT:
                direction = Vector2.up;
                break;
            case Dir.RIGHT:
                direction = Vector2.down;
                break;
            default:
                direction = Vector2.zero;
                break;
        }

        RaycastHit2D hit;
        float angle = who.transform.eulerAngles.z;
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector2 checkDir = new Vector2(cos * direction.x - sin * direction.y,
            sin * direction.x + cos * direction.y);
        hit = Physics2D.Raycast(who.transform.position, checkDir, 1f,
            1 << LayerMask.NameToLayer("Wall"));
        Debug.DrawRay(who.transform.position, checkDir, Color.blue, 1f);
        if (!hit)
            return true;
        else
            return false;

    }

    public void SetHoler(AIHolder holder)
    {
        this.holder = holder;
    }

    protected void LookAt(Dir direction, GameObject who)
    {
        float angle = 0f;
        switch (direction)
        {
            case Dir.BACKWARD:
                angle = -180f;
                break;
            case Dir.RIGHT:
                angle = -90f;
                break;
            case Dir.FORWARD:
                angle = 0f;
                break;
            case Dir.LEFT:
                angle = 90f;
                break;
            default:
                angle = 0f;
                break;
        }

        rotateCommand.Initialize(who.transform.eulerAngles.z + angle);
        rotateCommand.Execute(who);
    }

    protected void MoveCommand(GameObject who)
    {
        moveCommand.SetDirection(Vector2.right);
        moveCommand.Execute(who);
    }
}

