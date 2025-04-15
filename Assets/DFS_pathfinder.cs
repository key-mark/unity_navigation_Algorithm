using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS_pathfinder : MonoBehaviour
{
    public int width, height;//地图大小
    public int[,] dfs;//记录访问次序
    public bool[,] iswall;//记录障碍物信息
    public Pos startPos;
    public Pos endPos;
    public PathVisualizer visualizer;

    //表示探索的四个方向：上下左右
    private static readonly Pos[] directions = new Pos[]
    {
        new Pos(0, 1), new Pos(0, -1), new Pos(-1, 0), new Pos(1, 0)
    };

    private bool[,] visited;//记录哪些格子访问过
    private List<Pos> path;
    private bool found = false;//是否找到终点

    private void Start()
    {
       
    }

    public IEnumerator DFSWrapper()
    {
        dfs = new int[height, width];
        visited = new bool[height, width];
        path = new List<Pos>();
        found = false;

        //初始化搜索表格 dfs，所有值设为未访问（int.MaxValue）
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                dfs[y, x] = int.MaxValue;

        yield return StartCoroutine(DFS(startPos, 0));//启动DFS搜索，初始步数为0

        if (found)//如果找到路径，就逐步绘制路径动画
        {
            path.Reverse();
            visualizer.Refresh();
            foreach (var p in path)
            {
                visualizer.DrawFinalPathStep(p);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }


    private IEnumerator DFS(Pos cur, int step)
    {
        //越界，剪枝优化
        if (found || cur.x < 0 || cur.x >= width || cur.y < 0 || cur.y >= height)
            yield break;

        if (iswall[cur.y, cur.x] || visited[cur.y, cur.x])
            yield break;

        visited[cur.y, cur.x] = true;
        dfs[cur.y, cur.x] = step;

        if (visualizer != null)
        {
            visualizer.RefreshPath(ConvertToShortArray(dfs));
            yield return new WaitForSeconds(0.02f);
        }

        if (cur.Equals(endPos))
        {
            found = true;
            path.Add(cur);
            yield break;
        }

        foreach (var dir in directions)
        {
            Pos next = new Pos(cur.x + dir.x, cur.y + dir.y);
            yield return StartCoroutine(DFS(next, step + 1));
            if (found)
            {
                path.Add(cur);
                yield break;
            }
        }
    }


    /// <summary>
    /// 工具函数：将 int 类型的二维数组转换为 short 类型的二维数组，
    /// 用于适配可视化组件（PathVisualizer）中使用 short 类型地图的需求。
    /// 同时将 int.MaxValue 映射为 short.MaxValue，表示“不可达”或“未访问”
    /// </summary>
    /// <param name="source">原始的 int 类型的路径图数组</param>
    /// <returns>转换后的 short 类型数组</returns>
    private short[,] ConvertToShortArray(int[,] source)
    {
        // 获取原数组的尺寸
        short[,] result = new short[source.GetLength(0), source.GetLength(1)];

        for (int i = 0; i < source.GetLength(0); i++)
        {
            for (int j = 0; j < source.GetLength(1); j++)
            {
                // 如果原数组中是 int.MaxValue，代表还未访问，用 short.MaxValue 表示
                // 否则转换为 short 类型的距离
                result[i, j] = source[i, j] == int.MaxValue ? short.MaxValue : (short)source[i, j];
            }
        }

        return result;
    }


    private int[,] ConvertWallMap(bool[,] iswall, Pos start, Pos end)
    {
        int h = iswall.GetLength(0);
        int w = iswall.GetLength(1);
        int[,] map = new int[h, w];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                map[y, x] = iswall[y, x] ? 1 : 0;
            }
        }
        map[start.y, start.x] = 8;
        map[end.y, end.x] = 9;
        return map;
    }
}
