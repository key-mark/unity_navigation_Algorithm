using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// Pos代替Vector2，代表位置。整数
public struct Pos
{
    public int x;
    public int y;
    // 构造函数：从另一个 Pos 构造
    public Pos(Pos p)
    {
        x = p.x;
        y = p.y;
    }
    // 构造函数：指定 x 和 y
    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    // 判断两个 Pos 是否相等
    public bool Equals(Pos p)
    {
        return x == p.x && y == p.y;
    }
    // 可选：重写 ToString() 方便调试
    public override string ToString()
    {
        return $"({x}, {y})";
    }
}

public class BFS_pathfinder : MonoBehaviour
{
    public int width, height;
    public int[,] bfs;
    public bool[,] iswall;
    public Pos startPos;
    public Pos endPos;
    public PathVisualizer visualizer;

    private void Start()
    {
        
    }

    private static readonly Pos[] directions = new Pos[]
    {
        new Pos(0, 1), new Pos(0, -1), new Pos(-1, 0), new Pos(1, 0)
    };

    public IEnumerator BFS()
    {
        bfs = new int[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                bfs[y, x] = int.MaxValue;

        List<Pos> queue = new List<Pos>();
        bfs[startPos.y, startPos.x] = 0;
        queue.Add(startPos);

        while (queue.Count > 0)
        {
            Pos cur = queue[0];
            queue.RemoveAt(0);
            int curStep = bfs[cur.y, cur.x];

            // 如果已到终点
            if (cur.Equals(endPos))
                break;

            foreach (var dir in directions)
            {
                int nx = cur.x + dir.x;
                int ny = cur.y + dir.y;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    continue;
                if (iswall[ny, nx])
                    continue;
                if (bfs[ny, nx] != int.MaxValue)
                    continue;

                bfs[ny, nx] = curStep + 1;
                queue.Add(new Pos(nx, ny));
            }

            // 刷新路径（可视化）并暂停一帧
            if (visualizer != null)
            {
               visualizer.RefreshPath(ConvertToShortArray(bfs));
            }
                
            yield return new WaitForSeconds(0.05f);
        }
        // 回溯路径动画（搜索完成后）
        if (bfs[endPos.y, endPos.x] != int.MaxValue)
        {
            yield return StartCoroutine(ShowFinalPath());
            Debug.Log("1212");
        }
        yield return null;
    }
    private IEnumerator ShowFinalPath()
    {
        List<Pos> path = new List<Pos>();
        Pos cur = endPos;
        path.Add(cur);
        int curStep = bfs[cur.y, cur.x];

        while (curStep > 0)
        {
            foreach (var dir in directions)
            {
                int nx = cur.x + dir.x;
                int ny = cur.y + dir.y;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    continue;
                if (bfs[ny, nx] == curStep - 1)
                {
                    cur = new Pos(nx, ny);
                    path.Add(cur);
                    curStep--;
                    break;
                }
            }
        }

        path.Reverse();
        visualizer.Refresh();
        foreach (var p in path)
        {
            visualizer.DrawFinalPathStep(p); // 一步步显示路径
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 工具函数：将 int[,] 转为 short[,] 方便可视化兼容
    private short[,] ConvertToShortArray(int[,] source)
    {
        short[,] result = new short[source.GetLength(0), source.GetLength(1)];
        for (int i = 0; i < source.GetLength(0); i++)
            for (int j = 0; j < source.GetLength(1); j++)
                result[i, j] = source[i, j] == int.MaxValue ? short.MaxValue : (short)source[i, j];
        return result;
    }

    //工具函数：将文件中的iswall等参数，转变为int型地图变量
    private int[,] ConvertWallMap(bool[,] iswall, Pos start, Pos end)
    {
        int h = iswall.GetLength(0);
        int w = iswall.GetLength(1);
        int[,] map = new int[h, w];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (iswall[y, x])
                    map[y, x] = 1; // 墙体
                else
                    map[y, x] = 0; // 空地
            }
        }
        map[start.y, start.x] = 8; // 起点
        map[end.y, end.x] = 9;     // 终点
        return map;
    }

}

