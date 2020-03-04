using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUnitCommand : MonoBehaviour
{
    Space space;
    Vector2 dir;
    MoveHandler mh;

    private void Awake()
    {
        mh = GetComponent<MoveHandler>();
    }

    public MoveUnitCommand(Vector2 dir, Space space = Space.World)
    {
        this.dir = dir;
        this.space = space;
    }

    private void Update()
    {
        Execute();
    }

    void Execute()
    {
        if (space == Space.World)
            mh.MoveToWorldDir(dir);
        else
            mh.MoveToSelfDir(dir);

        dir = Vector2.zero;
    }

    public void MoveTo(Vector2 directioin)
    {   
        //이동 명령이 들어오면 적어도 한번은 수행
        if (directioin == Vector2.zero)
            return;

        this.dir = directioin;
    }
}



