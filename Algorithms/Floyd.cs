using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floyd
{
    private static bool IsNextToEachOther(TileMap.Node node1, TileMap.Node node2)
    {
        int subX = node1.X - node2.X;
        int subY = node1.Y - node2.Y;
        if (Mathf.Abs(subX) + Mathf.Abs(subY) == 1)
            return true;
        return false;
    }
        private static void InitDistance(TileMap map)
    {
        for (int i = 0; i < map.MapSizeX; i++)
        {
            for (int j = 0; j < map.MapSizeY; j++)
            {
                if (map.Graph[i, j].Cost == 0)
                {
                    continue;
                }
                for (int k = 0; k < map.MapSizeX; k++)
                {
                    for (int l = 0; l < map.MapSizeY; l++)
                    {
                        if (map.Graph[k, l].Cost == 0)
                        {
                            map.Graph[i, j].CostTo[k, l] = 100;
                        }
                        else if (i == k && j == l)
                            map.Graph[i, j].CostTo[k, l] = 0;
                        else if (IsNextToEachOther(map.Graph[i, j], map.Graph[k, l]))
                        {
                            map.Graph[i, j].CostTo[k, l] = map.Graph[k, l].Cost;
                        }
                        else
                            map.Graph[i, j].CostTo[k, l] = 999;
                    }
                }
            }
        }
    }
    public static void CalculateHeuristicForAStar(TileMap map)
    {
        InitDistance(map);
        for (int m = 0; m < map.MapSizeX; m++)
        {
            for (int n = 0; n < map.MapSizeY; n++)
            {
                if (map.Graph[m, n].Cost == 0)
                {
                    continue;
                }
                for (int i = 0; i < map.MapSizeX; i++)
                {
                    for (int j = 0; j < map.MapSizeY; j++)
                    {
                        if (map.Graph[i, j].Cost == 0
                            || (i == m && j == n))
                        {
                            continue;
                        }
                        for (int k = 0; k < map.MapSizeX; k++)
                        {
                            for (int l = 0; l < map.MapSizeY; l++)
                            {
                                if (map.Graph[k, l].Cost == 0
                                    || (k == m && l == n)
                                    || (k == i && l == j))
                                {
                                    continue;
                                }
                                map.Graph[i, j].CostTo[k, l] = Mathf.Min(map.Graph[i, j].CostTo[k, l],
                                    map.Graph[i, j].CostTo[m, n] + map.Graph[m, n].CostTo[k, l]);
                            }
                        }
                    }
                }
            }
        }
    }
}
