using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab_start, prefab_end, prefab_path,prefab_final_path;
    public GameObject pathParent;
    public int H, W;
    public int[,] map;
    public const int START = 8;
    public const int END = 9;

    public void Refresh()
    {
        GameObject[] all_go = GameObject.FindGameObjectsWithTag("Path");
        foreach(var go in all_go)
        {
            Destroy(go);
        }
        for(int i = 0;i<H;i++)
        {
            for(int j =0;j<W;j++)
            {
                if (map[i,j] == START)
                {
                    var go = Instantiate(prefab_start, new Vector3(j, 0.5f, i), Quaternion.identity, pathParent.transform);
                    go.tag = "Path";
                }
                else if(map[i,j] == END)
                {
                    var go = Instantiate(prefab_end, new Vector3(j, 0.5f, i), Quaternion.identity, pathParent.transform);
                    go.tag = "Path";
                }
            }
        }
    }
    public void RefreshPath(short[,]bfs)
    {
        Refresh();
        for (int i = 0; i < H; i++)
        {
            for (int j = 0; j < W; j++)
            {
                if (map[i,j] == 0 && bfs[i,j]!=short.MaxValue)
                {
                    var go = Instantiate(prefab_path, new Vector3(j, 0.5f, i), Quaternion.identity, pathParent.transform);
                    go.tag = "Path";
                   // Debug.Log("1212");
                }
            }
        }
    }
    public void DrawFinalPathStep(Pos p)
    {
        var go = Instantiate(prefab_final_path, new Vector3(p.x, 0.3f, p.y), Quaternion.identity, pathParent.transform);
        //go.GetComponent<Renderer>().material.color = Color.yellow;
        go.tag = "Path";
    }

}
