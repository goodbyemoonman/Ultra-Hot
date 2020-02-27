using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAI : AIBase {
    List<Node> openNodes;
    List<Node> closedNodes;
    List<Node> nodePool;
    List<Node> path;
    Vector2Int mapSize;

    public new void Awake()
    {
        base.Awake();
        openNodes = new List<Node>();
        closedNodes = new List<Node>();
        nodePool = new List<Node>();
    }

    public override void Do(GameObject who)
    {

    }

    void Seek(Vector2Int startPos, Vector2Int targetPos)
    {
        InitNodeList();
        Node sn = GetNewNode(startPos.x, startPos.y);
        openNodes.Add(sn);
        
        while(openNodes.Count > 0)
        {
            Node curNode = GetCurrentNode(openNodes);
            closedNodes.Add(curNode);

            //마무리 단계
            if(curNode.pos == targetPos)
            {
                while(curNode.pos != startPos)
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
        //맵 범위 밖인 경우
        if (checkPos.x < 0 || checkPos.y < 0 || mapSize.x <= checkPos.x || mapSize.y <= checkPos.y)
            return;
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

        if(moveCost < n.G || !openNodes.Contains(n))
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
        foreach(Node n in nodes)
        {
            if (n.pos == pos)
                return n;
        }
        return null;
    }

    Node GetCurrentNode(List<Node> nodes)
    {
        if (nodes.Count == 0)
            return null;
        Node result = nodes[0];
        foreach(Node n in nodes)
        {
            if (n.F <= result.F && n.H < result.H)
                result = n;
        }
        nodes.Remove(result);
        return result;
    }

    public void SetTargetTf(Transform target)
    {

    }

    public override void Initialize(GameObject who)
    {
        InitNodeList();
    }

    void InitNodeList()
    {
        mapSize = WorldMaker.Instance.GetTileMapSize();
        foreach (Node n in openNodes)
        {
            nodePool.Add(n);
            n.Init();
            openNodes.Remove(n);
        }
        openNodes.Clear();
        foreach (Node n in closedNodes)
        {
            nodePool.Add(n);
            n.Init();
            closedNodes.Remove(n);
        }
        closedNodes.Clear();
        foreach (Node n in path)
        {
            nodePool.Add(n);
            n.Init();
            path.Remove(n);
        }
        path.Clear();
    }

    Node GetNewNode(int x, int y)
    {
        Node n;
        if(nodePool.Count > 0)
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
}

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

