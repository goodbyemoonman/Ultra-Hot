using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAlgorithm {
    List<Node> openNodes;
    List<Node> closedNodes;
    List<Node> nodePool;
    List<Node> finalPath;

    public SeekAlgorithm(){
        openNodes = new List<Node>();
        closedNodes = new List<Node>();
        nodePool = new List<Node>();
        finalPath = new List<Node>();
        InitNodeLists();
    }

    void InitNodeLists()
    {
        InitNodeList(openNodes);
        InitNodeList(closedNodes);
        InitNodeList(finalPath);
    }

    void InitNodeList(List<Node> nodes)
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            if (nodePool.Contains(nodes[i]))
                continue;

            nodePool.Add(nodes[i]);
            nodes[i].Init();
            nodes.Remove(nodes[i]);
        }
        nodes.Clear();
    }

    Node GetNewNode(Vector2Int v)
    {
        Node n;
        if (nodePool.Count > 0)
        {
            n = nodePool[0];
            nodePool.Remove(n);
            n.Init();
            n.coord = v;
        }
        else
            n = new Node(v);

        return n;
    }
    Node GetNewNode(int x, int y)
    {
        return GetNewNode(new Vector2Int(x, y));
    }

    Node ChooseCurrentNode(List<Node> nodes)
    {
        if (nodes.Count == 0)
            return null;
        Node result = nodes[0];
        for(int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].F <= result.F && nodes[i].H < result.H)
                result = nodes[i];
        }
        nodes.Remove(result);
        return result;
    }

    void CheckAddToOpenNodes(Vector2Int checkCrd, Node curNode, Vector2Int goal)
    {
        if (IsWall(checkCrd))
            return;
        if (FindNodeWithCrd(checkCrd, closedNodes) != null)
            return;
        if (IsWall(new Vector2Int(checkCrd.x, curNode.coord.y)) ||
            IsWall(new Vector2Int(curNode.coord.x, checkCrd.y)))
            return;

        Node n = FindNodeWithCrd(checkCrd, openNodes);
        if (n == null)
            n = GetNewNode(checkCrd);

        int moveCost = curNode.G +
            GetCost(checkCrd, curNode.coord);

        if(moveCost < n.G || !openNodes.Contains(n))
        {
            n.G = moveCost;
            n.H = GetH(n.coord, goal);
            n.parent = curNode;
        }

        if (!openNodes.Contains(n))
        { 
            openNodes.Add(n);
        }
    }

    int GetH(Vector2Int start, Vector2Int goal)
    {
        return ((Mathf.Abs(start.x - goal.x) + Mathf.Abs(start.y - goal.y)) * 10);
    }

    Node FindNodeWithCrd(Vector2Int crd, List<Node> nodes)
    {
        for(int i= 0; i < nodes.Count; i++)
        {
            if (nodes[i].coord == crd)
                return nodes[i];
        }
        return null;
    }

    int GetCost(Vector2Int a, Vector2Int b)
    {
        return (a.x != b.x && a.y != b.y) ? 14 : 10;
    }

    void seek(Vector2Int start, Vector2Int goal)
    {
        InitNodeLists();
        Node n = GetNewNode(start);
        openNodes.Add(n);

        while(openNodes.Count > 0)
        {
            Node curN = ChooseCurrentNode(openNodes);
            closedNodes.Add(curN);

            if(curN.coord == goal)
            {
                while(curN.parent != null)
                {
                    finalPath.Add(curN);
                    curN = curN.parent;
                }
                finalPath.Add(curN);
                finalPath.Reverse();

                return;
            }
            
            CheckAddToOpenNodes(curN.coord + Vector2Int.left + Vector2Int.down, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.down, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.right + Vector2Int.down, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.left, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.right, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.left + Vector2Int.up, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.up, curN, goal);
            CheckAddToOpenNodes(curN.coord + Vector2Int.right + Vector2Int.up, curN, goal);
        }

    }

    bool IsWall(Vector2Int crd)
    {
        int wallLayer = 1 << LayerMask.NameToLayer("Wall");
        if (Physics2D.OverlapCircle(crd, 0.4f, wallLayer))
            return true;
        return
            false;
    }

    public List<Vector2Int> GetPath(Vector2Int start, Vector2Int goal)
    {
        seek(start, goal);

        List<Vector2Int> result = new List<Vector2Int>();

        for(int i = 0;i < finalPath.Count; i++)
        {
            result.Add(finalPath[i].coord);
        }

        return result;
    }
}

class Node
{
    public Vector2Int coord;
    public int G, H;
    public int F { get { return G + H; } }
    public Node parent;
    public Node(Vector2Int v)
    {
        coord = v;
    }
    public void Init()
    {
        coord = Vector2Int.zero;
        G = H = 0;
        parent = null;
    }
}