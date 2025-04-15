using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class mapnavi : MonoBehaviour
{
    const int Height = 20;
    const int Width = 30;
    int[,] map = new int[Height, Width];

    public GameObject prefab_wall;

    // ✅ 不在这里调用 FindObjectOfType
    BFS_pathfinder pathfinder;
    DFS_pathfinder dFS_Pathfinder;
    AStar_pathfinder aStar_Pathfinder;
    PathVisualizer visualizer;

    const int START = 8;
    const int END = 9;
    const int WALL = 1;

    void Start()
    {
        // ✅ 正确地在 Start 里查找
        pathfinder = FindObjectOfType<BFS_pathfinder>();
        dFS_Pathfinder = FindObjectOfType<DFS_pathfinder>();
        aStar_Pathfinder = FindObjectOfType<AStar_pathfinder>();
        visualizer = FindObjectOfType<PathVisualizer>();

        if (pathfinder == null || visualizer == null || dFS_Pathfinder==null ||aStar_Pathfinder ==null)
        {
            Debug.LogError("Pathfinder 或 PathVisualizer 未找到！");
            return;
        }

        ReadMapFile();
        InitMap0();

        //pathfinder.width = Width;
        //pathfinder.height = Height;
        //pathfinder.iswall = ConvertToBoolMap(map);
        //pathfinder.startPos = FindStart(map);
        //pathfinder.endPos = FindEnd(map);

        //dFS_Pathfinder.width = Width;
        //dFS_Pathfinder.height = Height;
        //dFS_Pathfinder.iswall = ConvertToBoolMap(map);
        //dFS_Pathfinder.startPos = FindStart(map);
        //dFS_Pathfinder.endPos = FindEnd(map);

        aStar_Pathfinder.width = Width;
        aStar_Pathfinder.height = Height;
        aStar_Pathfinder.iswall = ConvertToBoolMap(map);
        aStar_Pathfinder.startPos = FindStart(map);
        aStar_Pathfinder.endPos = FindEnd(map);

        pathfinder.visualizer = visualizer;
        visualizer.map = map;
        visualizer.H = Height;
        visualizer.W = Width;

        //StartCoroutine(pathfinder.BFS());
        //StartCoroutine(dFS_Pathfinder.DFSWrapper());
        StartCoroutine(aStar_Pathfinder.AStar());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static bool[,] ConvertToBoolMap(int[,] map)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);
        bool[,] result = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                result[y, x] = (map[y, x] == 1); // 1 是墙
            }
        }
        return result;
    }
    public static Pos FindStart(int[,] map)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[y, x] == 8)
                    return new Pos(x, y);
            }
        }
        Debug.LogError("Start position (8) not found in map!");
        return new Pos(0, 0); // 默认值
    }
    public static Pos FindEnd(int[,] map)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[y, x] == 9)
                    return new Pos(x, y);
            }
        }
        Debug.LogError("End position (9) not found in map!");
        return new Pos(0, 0); // 默认值
    }


    void InitMap0()
    {
        var walls = new GameObject();
        walls.name = "walls";
        walls.transform.parent = null;
        walls.transform.position = Vector3.zero;
        for (int i=0;i<Height;i++)
        {
            for(int j=0;j<Width;j++)
            {
                if(map[i,j] == WALL)
                {
                    var go = Instantiate(prefab_wall, new Vector3(j*1,0.5f,i*1),Quaternion.identity,walls.transform);
                }
            }
        }
    }
    //编写文本文件来定义地图的方法
    public void ReadMapFile()
    {
        string path = Application.dataPath + "//" + "map.txt";
        if(!File.Exists(path))
        {
            return;
        }
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        StreamReader read = new StreamReader(fs, Encoding.Default);

        string strReadline = "";
        int y = 0;

        read.ReadLine();
        strReadline = read.ReadLine();

        while (strReadline != null&&y<Height) 
        {
            for(int x =0;x < Width && x < strReadline.Length; ++x)
            {
                int t;
                switch(strReadline[x])
                {
                    case '1':
                        t = 1;
                        break;
                    case '8':
                        t = 8;
                        break;
                    case '9':
                        t = 9;
                        break;
                    default:
                        t = 0;
                        break;
                }
                map[y,x] = t;
            }
            y += 1;
            strReadline = read.ReadLine() ;
        }
        read.Dispose();
        fs.Close();
    }
}
