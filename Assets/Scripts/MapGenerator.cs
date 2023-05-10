using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int itemSpace = 10;
    public int itemCountInMap = 5;
    int mapSize;
    enum TrackPos { Left = -1, Center = 0, Right = 1 }

    public List<GameObject> ObstaclesTopPrefab;
    public List<GameObject> ObstaclesBottomPrefab;
    public List<GameObject> ObstaclesFullPrefab;

    public float laneOffset = 1.5f;
    public List<GameObject> maps = new List<GameObject>();
    public List<GameObject> activeMaps = new List<GameObject>();

    public static MapGenerator instance;

    private void Awake()
    {
        instance = this;

        mapSize = itemCountInMap * itemSpace;
        maps.Add(MakeMap2());
        maps.Add(MakeMap3());
        foreach (var map in maps)
            map.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {
        if (RoadGenerator.instance.speed == 0) return;

        foreach (var map in activeMaps)
            map.transform.position -= new Vector3(0, 0, RoadGenerator.instance.speed * Time.deltaTime);
        if (activeMaps[0].transform.position.z < -mapSize)
        {
            RemoveFirstActiveMap();
            AddActiveMap();
        }
    }

    public void ResetMaps()
    {
        while (activeMaps.Count > 0)
            RemoveFirstActiveMap();
        AddActiveMap();
        AddActiveMap();
    }

    void RemoveFirstActiveMap()
    {
        activeMaps[0].SetActive(false);
        maps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    void AddActiveMap()
    {
        int r = Random.Range(0, maps.Count);
        GameObject go = maps[r];
        go.SetActive(true);
        foreach (Transform child in go.transform)
            child.gameObject.SetActive(true);
        go.transform.position = activeMaps.Count > 0 ?
                                activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * mapSize :
                                new Vector3(0, 0, 10);
        maps.RemoveAt(r);
        activeMaps.Add(go);
    }

    private GameObject MakeMap1()
    {
        GameObject result = new GameObject("Map1");
        result.transform.parent = this.transform;
        for (int i = 0; i < itemCountInMap; i++)
        {
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;

            if (true) { trackPos = (TrackPos)Random.Range(-1, 2); obstacle = RandomObstacle(); }

            Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
            if (obstacle != null)
                Instantiate(obstacle, obstaclePos, Quaternion.identity, result.transform);
        }
        return result;
    }

    private GameObject MakeMap2()
    {
        GameObject result = new GameObject("Map1");
        result.transform.parent = this.transform;
        for (int i = 0; i < itemCountInMap; i++)
        {
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;

            if (i == 0) { trackPos = TrackPos.Center; obstacle = ObstaclesBottomPrefab[Random.Range(0, ObstaclesBottomPrefab.Count)]; }
            if (i == 1) { trackPos = TrackPos.Center; obstacle = ObstaclesTopPrefab[Random.Range(0, ObstaclesTopPrefab.Count)]; }
            if (i == 2) { trackPos = TrackPos.Left; obstacle = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; }
            if (i == 3) { trackPos = TrackPos.Right; obstacle = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; }
            if (i == 4) { trackPos = TrackPos.Center; obstacle = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; }

            Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
            if (obstacle != null)
                Instantiate(obstacle, obstaclePos, Quaternion.identity, result.transform);
        }
        return result;
    }

    private GameObject MakeMap3()
    {
        GameObject result = new GameObject("Map1");
        result.transform.parent = this.transform;
        for (int i = 0; i < itemCountInMap; i++)
        {
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;

            if (i == 0) { trackPos = TrackPos.Left; obstacle = ObstaclesBottomPrefab[Random.Range(0, ObstaclesBottomPrefab.Count)]; }
            if (i == 1) { trackPos = TrackPos.Left; obstacle = ObstaclesTopPrefab[Random.Range(0, ObstaclesTopPrefab.Count)]; }
            if (i == 2) { trackPos = TrackPos.Right; obstacle = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; }
            if (i == 3) { trackPos = TrackPos.Right; obstacle = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; }
            if (i == 4) { trackPos = TrackPos.Left; obstacle = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; }

            Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
            if (obstacle != null)
                Instantiate(obstacle, obstaclePos, Quaternion.identity, result.transform);
        }
        return result;
    }


    GameObject RandomObstacle()
    {
        GameObject res = ObstaclesBottomPrefab[0];
        var i = Random.Range(0, 3);
        switch (i)
        {
            case 0: res = ObstaclesBottomPrefab[Random.Range(0, ObstaclesBottomPrefab.Count)]; break;
            case 1: res = ObstaclesFullPrefab[Random.Range(0, ObstaclesFullPrefab.Count)]; break;
            case 2: res = ObstaclesTopPrefab[Random.Range(0, ObstaclesTopPrefab.Count)]; break;
            default:
                break;
        }
        return res;
    }
}
