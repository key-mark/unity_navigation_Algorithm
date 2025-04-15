using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AStar_pathfinder : MonoBehaviour
{
    public int width, height;
    public int[,] astar;
    public bool[,] iswall;
    public Pos startPos;
    public Pos endPos;
    public PathVisualizer visualizer;

    private static readonly Pos[] directions = new Pos[]
    {
        new Pos(0, 1), new Pos(0, -1), new Pos(-1, 0), new Pos(1, 0)
    };

    //记录当前节点位置、代价、启发、父节点
    private class Node
    {
        public Pos pos;
        public int g; // 从起点到当前点的代价
        public int h; // 启发式：当前点到终点的估计代价
        //每次你访问 f，它动态返回当前 g 和 h 的和，而不是在某个时刻就固定值。
        public int f => g + h; // 总估价
        public Node parent;

        public Node(Pos pos, int g, int h, Node parent)
        {
            this.pos = pos;//位置
            this.g = g;//起点到此点的距离
            this.h = h;//此点到终点的距离
            this.parent = parent;//用于回溯路径
        }
    }

    private void Start()
    {
        //StartCoroutine(AStar());
    }

    public IEnumerator AStar()
    {
        astar = new int[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                astar[y, x] = int.MaxValue;

        List<Node> openList = new List<Node>();//待探索节点列表（按照 f = g + h 排序）
        HashSet<Pos> closedSet = new HashSet<Pos>();//已探索的节点集合

        Node startNode = new Node(startPos, 0, Heuristic(startPos, endPos), null);
        openList.Add(startNode);
        astar[startPos.y, startPos.x] = 0;

        while (openList.Count > 0)
        {
            // 按 f 值排序，选出最小的
            openList.Sort((a, b) => a.f.CompareTo(b.f));//排序后，openList[0] 就是 f 值最小的那个节点。
            Node current = openList[0];
            openList.RemoveAt(0);

            if (closedSet.Contains(current.pos))//检测到重复的，跳过
                continue;

            closedSet.Add(current.pos);
            astar[current.pos.y, current.pos.x] = current.g;

            if (visualizer != null)
                visualizer.RefreshPath(ConvertToShortArray(astar));

            yield return new WaitForSeconds(0.02f);

            if (current.pos.Equals(endPos))
            {
                yield return StartCoroutine(ShowFinalPath(current));
                yield break;
            }

            foreach (var dir in directions)
            {
                Pos nextPos = new Pos(current.pos.x + dir.x, current.pos.y + dir.y);//计算下一个坐标

                //边界检查和障碍判断
                if (nextPos.x < 0 || nextPos.x >= width || nextPos.y < 0 || nextPos.y >= height)
                    continue;
                if (iswall[nextPos.y, nextPos.x] || closedSet.Contains(nextPos))
                    continue;

                int tentativeG = current.g + 1;//+1后作为新节点的g值

                Node existing = openList.Find(n => n.pos.Equals(nextPos));//用 Find 检查 openList 中是否已有相同位置的节点。
                                                                          //遍历 openList 中的每一个 Node n，如果 n.pos 和 nextPos 是同一个位置，就返回这个节点。
                if (existing == null || tentativeG < existing.g)
                {
                    Node nextNode = new Node(nextPos, tentativeG, Heuristic(nextPos, endPos), current);
                    if (existing == null)
                        openList.Add(nextNode);
                    else
                    {
                        existing.g = tentativeG;
                        existing.parent = current;
                    }
                }
            }
        }

        Debug.Log("未找到路径");
    }

    private IEnumerator ShowFinalPath(Node endNode)
    {
        List<Pos> path = new List<Pos>();
        Node cur = endNode;
        while (cur != null)
        {
            path.Add(cur.pos);
            cur = cur.parent;
        }

        path.Reverse();
        visualizer.Refresh();
        foreach (var p in path)
        {
            visualizer.DrawFinalPathStep(p);
            yield return new WaitForSeconds(0.05f);
        }
    }

    //使用曼哈顿距离
    private int Heuristic(Pos a, Pos b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // 曼哈顿距离
    }

    private short[,] ConvertToShortArray(int[,] source)
    {
        short[,] result = new short[source.GetLength(0), source.GetLength(1)];
        for (int i = 0; i < source.GetLength(0); i++)
            for (int j = 0; j < source.GetLength(1); j++)
                result[i, j] = source[i, j] == int.MaxValue ? short.MaxValue : (short)source[i, j];
        return result;
    }
}
