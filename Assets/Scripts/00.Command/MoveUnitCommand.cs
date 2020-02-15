using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUnitCommand : ICommand
{
    Space space;
    Vector2 dir;

    public MoveUnitCommand(Vector2 dir, Space space = Space.World)
    {
        this.dir = dir;
        this.space = space;
    }

    public void Execute(GameObject obj)
    {
        if (dir == null)
        {
            Debug.Log("direction missing");
            return;
        }
        if (space == Space.World)
            obj.SendMessage("MoveToWorldDir", dir);
        else
            obj.SendMessage("MoveToSelfDir", dir);

        dir = Vector2.zero;
    }

    public void SetDirection(Vector2 dir)
    {
        if (dir == Vector2.zero)
            return;
        this.dir = dir;
    }
}
