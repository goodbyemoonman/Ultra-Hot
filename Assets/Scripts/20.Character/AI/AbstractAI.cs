using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAI {
    protected GameObject who;
    protected ActHandler actHandler;
    protected MoveHandler moveHandler;

    protected BoundaryCheckAlgorithm bca;
    protected SeekAlgorithm sa;
    protected List<Vector2> path;
    protected Vector3 targetPos;

    public abstract bool CheckAIChange(AIHolder aiHolder);
    public abstract bool Check(AIHolder aiHolder);
    public abstract void Do();
}