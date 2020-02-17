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
        if (space == Space.World)
            obj.SendMessage("MoveToWorldDir", dir);
        else
            obj.SendMessage("MoveToSelfDir", dir);

        dir = Vector2.zero;
    }

    public void SetDirection(Vector2 input)
    {   
        //이동 명령이 들어오면 적어도 한번은 수행
        if (input == Vector2.zero)
            return;

        this.dir = input;
    }
}



