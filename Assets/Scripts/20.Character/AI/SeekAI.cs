using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAI : AIBase
{
    Transform target;
    List<Node> openNodes;
    List<Node> closedNodes;
    List<Node> nodePool;
    List<Node> path;

    public override void Do(GameObject who)
    {
        if(path.Count == 0){
            Seek(GetIntPos(who.transform.position), GetIntPos(target.position));

            if(path.Count == 0)
            {
                //그 곳까지 가는 방법이 없음.
                //AI를 배회 AI로.
                Debug.Log("길이 없다");
            }
        }
        else
        {
            mh.CallBackMoveDir(path[0].pos, "Arrive");
        }
    }

    Vector2Int GetIntPos(Vector2 input)
    {
        Vector2Int result = new Vector2Int(
            Mathf.RoundToInt(input.x), 
            Mathf.RoundToInt(input.y));

        return result;
    }

    void Seek(Vector2Int startPos, Vector2Int targetPos)
    {
        InitNodeList();
        Node sn = GetNewNode(startPos.x, startPos.y);
        openNodes.Add(sn);

        while (openNodes.Count > 0)
        {
            Node curNode = GetCurrentNode(openNodes);
            closedNodes.Add(curNode);

            //마지막 부분. 길을 다 찾은 상태라면
            if (curNode.pos == targetPos)
            {
                while (curNode.parent != null)
                {
                    path.Add(curNode);
                    curNode = curNode.parent;
                }
                path.Add(curNode);
                path.Reverse();

                return;
            }

            Vector2Int dir = new Vector2Int(-1, -1);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(-1, 0);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(-1, 1);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(0, -1);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(0, 1);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(1, -1);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(1, 0);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
            dir = new Vector2Int(1, 1);
            CheckAddToOpenNodes(curNode.pos + dir, curNode, targetPos);
        }
    }

    void CheckAddToOpenNodes(Vector2Int checkPos, Node curNode, Vector2Int targetPos)
    {
        //벽인 경우
        if (WorldMaker.Instance.IsWall(checkPos))
            return;
        //닫힌노드인 경우
        if (GetNodeExist(checkPos, closedNodes) != null)
            return;
        //벽 사이를 뚫거나 벽에 막히는경우
        if (WorldMaker.Instance.IsWall(new Vector2Int(checkPos.x, curNode.pos.y)) ||
            WorldMaker.Instance.IsWall(new Vector2Int(curNode.pos.x, checkPos.y)))
            return;

        Node n = GetNodeExist(checkPos, openNodes);
        if (n == null)
            n = GetNewNode(checkPos.x, checkPos.y);

        int moveCost = curNode.G +
            (IsDiagonal(checkPos, curNode.pos) ? 14 : 10);

        if (moveCost < n.G || !openNodes.Contains(n))
        {
            n.G = moveCost;
            n.H = GetH(n.pos, targetPos);
            n.parent = curNode;

            openNodes.Add(n);
        }
    }

    bool IsDiagonal(Vector2Int a, Vector2Int b)
    {
        return (a.x != b.x && a.y != b.y);
    }

    Node GetNodeExist(Vector2Int pos, List<Node> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].pos == pos)
                return nodes[i];
        }
        return null;
    }

    Node GetCurrentNode(List<Node> nodes)
    {
        if (nodes.Count == 0)
            return null;
        Node result = nodes[0];
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].F <= result.F && nodes[i].H < result.H)
                result = nodes[i];
        }
        nodes.Remove(result);
        return result;
    }

    public void SetTargetTf(Transform _target)
    {
        target = _target;
    }

    public override void Initialize(GameObject who)
    {
        mh = who.GetComponent<MoveHandler>();
        openNodes = new List<Node>();
        closedNodes = new List<Node>();
        nodePool = new List<Node>();
        path = new List<Node>();
        InitNodeList();
    }

    void InitNodeList()
    {
        for (int i = 0; i < openNodes.Count; i++)
        {
            if (nodePool.Contains(openNodes[i]))
                continue;
            nodePool.Add(openNodes[i]);
            openNodes[i].Init();
            openNodes.Remove(openNodes[i]);
        }
        openNodes.Clear();
        for (int i = 0; i < closedNodes.Count; i++)
        {
            if (nodePool.Contains(closedNodes[i]))
                continue;
            nodePool.Add(closedNodes[i]);
            closedNodes[i].Init();
            closedNodes.Remove(closedNodes[i]);
        }
        closedNodes.Clear();
        for (int i = 0; i < path.Count; i++)
        {
            if (nodePool.Contains(path[i]))
                continue;
            nodePool.Add(path[i]);
            path[i].Init();
            path.Remove(path[i]);
        }
        path.Clear();
    }

    Node GetNewNode(int x, int y)
    {
        Node n;
        if (nodePool.Count > 0)
        {
            n = nodePool[0];
            nodePool.Remove(n);
            n.Init();
            n.x = x;
            n.y = y;
        }
        else
        {
            n = new Node(x, y);
        }

        return n;
    }

    int GetH(Vector2Int origin, Vector2Int target)
    {
        int result = Mathf.Abs(origin.x - target.x) + Mathf.Abs(origin.y - target.y);
        result *= 10;
        return result;
    }

    public void Arrive(GameObject who)
    {
        Debug.Log("도착");
        if (path.Count <= 1)
        {
            path.Clear();
            who.SendMessage("EKeyDown");
            //AI를 배회 AI로.
        }
        else
            path.RemoveAt(0);
    }
}

[System.Serializable]
class Node
{
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Node parent;
    public int x, y, G, H;
    public int F { get { return G + H; } }
    public void Init()
    {
        x = y = G = H = 0;
        parent = null;
    }
    public Vector2Int pos
    {
        get { return new Vector2Int(x, y); }
    }
}

