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
        if (Utility.IsWall(checkCrd))
            return;
        if (FindNodeWithCrd(checkCrd, closedNodes) != null)
            return;
        if (Utility.IsWall(new Vector2Int(checkCrd.x, curNode.coord.y)) ||
            Utility.IsWall(new Vector2Int(curNode.coord.x, checkCrd.y)))
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
    
    public List<Vector2> GetPath(Vector3 start, Vector3 goal)
    {
        seek(Utility.V3ToV2I(start), Utility.V3ToV2I(goal));

        List<Vector2> result = new List<Vector2>();

        for(int i = 0;i < finalPath.Count; i++)
        {
            result.Add(finalPath[i].coord);
        }
        result.Insert(0, start);
        result[result.Count - 1] = goal;
        result = PrettyPath(result);
        return result;
    }

    List<Vector2> PrettyPath(List<Vector2> path)
    {
        if (path.Count < 3)
            return path;
        List<Vector2> result = new List<Vector2>();
        result.Add(path[0]);
        path.Remove(path[0]);
        while (path.Count != 0)
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                if(i == 0)
                {
                    result.Add(path[0]);
                    path.Remove(path[0]);
                    break;
                }
                else if (CanMove(result[result.Count - 1], path[i]))
                {
                    result.Add(path[i]);
                    path.RemoveRange(0, i + 1);
                    break;
                }
            }
        }

        return result;
    }

    bool CanMove(Vector2 start, Vector2 end)
    {
        return !Physics2D.CircleCast(start, 0.4f, end - start, (end - start).magnitude, Utility.WallLayer);
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

public class BoundaryCheckAlgorithm
{
    Collider2D[] CheckBoundary(GameObject who, float radius, int layer)
    {
        return Physics2D.OverlapCircleAll(who.transform.position, radius, layer);
    }

    List<GameObject> ArrayToList(Collider2D[] cols, GameObject who)
    {
        List<GameObject> result = new List<GameObject>();

        for(int i = 0; i < cols.Length; i++)
        {
            //두 물체 사이에 벽이 없으면서
            //if(Utility.IsBlockWith(
            //    cols[i].transform.position,
            //    who.transform.position, Utility.WallLayer) == false)
            //{
            //    //who가 물체방향을 바라보는 것들만
            //    if (Utility.IsLookAt(who, cols[i].transform.position))
            result.Add(cols[i].gameObject);
            //}
        }

        return result;
    }

    void QuickSortByDistance(List<GameObject> gos, Vector3 center)
    {
        GameObject tmp;
        int pivot;
        Stack<Vector2Int> lvrvs = new Stack<Vector2Int>();

        CheckPushNewLvRv(lvrvs, 0, gos.Count - 1, gos, center);
        while (lvrvs.Count > 0)
        {
            Vector2Int lvrv = lvrvs.Pop();
            pivot = lvrv.x;
            int lv = pivot + 1;
            int rv = lvrv.y;

            while (lv < rv)
            {
                //pivot보다 큰 값까지 lv를 우측으로 이동
                while (
                    IsRightFartherThanLeft(
                        gos[lv].transform.position,
                        gos[pivot].transform.position, center)
                    &&
                    lv < rv
                    )
                {
                    lv++;
                }
                //pivot보다 작은 값까지 rv를 좌측으로 이동
                while (
                    IsRightFartherThanLeft(
                        gos[pivot].transform.position,
                        gos[rv].transform.position, center)
                    &&
                    lv <= rv)
                {
                    rv--;
                }

                //
                if (lv < rv)
                {
                    tmp = gos[rv];
                    gos[rv] = gos[lv];
                    gos[lv] = tmp;
                }
            }
            tmp = gos[pivot];
            gos[pivot] = gos[rv];
            gos[rv] = tmp;

            CheckPushNewLvRv(lvrvs, lvrv.x, rv - 1, gos, center);
            CheckPushNewLvRv(lvrvs, lv, lvrv.y, gos, center);

        }
    }

    bool IsRightFartherThanLeft(Vector3 lv, Vector3 rv, Vector3 center)
    {
        if ((lv - center).magnitude < (rv - center).magnitude)
            return true;
        else
            return false;
    }

    void CheckPushNewLvRv(Stack<Vector2Int> lvrvs, int lv, int rv, List<GameObject> gos, Vector3 center)
    {
        if (rv - lv + 1 > 2)
            lvrvs.Push(new Vector2Int(lv, rv));
        else if (rv - lv + 1 == 2)
        {
            if (IsRightFartherThanLeft(
                gos[rv].transform.position,
                gos[lv].transform.position, center))
            {
                GameObject tmp = gos[rv];
                gos[rv] = gos[lv];
                gos[lv] = tmp;
            }
        }

    }

    List<GameObject> CutOffAlreadyEquiped(List<GameObject> gos)
    {
        List<GameObject> result = new List<GameObject>();
        for(int i = 0; i < gos.Count; i++)
        {
            if (gos[i].transform.parent == null)
                result.Add(gos[i]);
        }

        return result;
    }

    public List<GameObject> GetObjListInSight(GameObject who, float radius, int layer)
    {
        List<GameObject> result = ArrayToList(CheckBoundary(who, radius, layer), who);

        QuickSortByDistance(result, who.transform.position);
        result = CutOffAlreadyEquiped(result);

        return result;
    }
}