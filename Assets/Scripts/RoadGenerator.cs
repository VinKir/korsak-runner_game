using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public static RoadGenerator instance;

    public int roadSegmentLength = 10;
    public List<GameObject> roadPrefabs;
    List<GameObject> currentRoads = new List<GameObject>();
    public float startSpeed = 10;
    public float speed = 10;
    public int maxRoadCount = 6;
    public int maxSpeed = 32;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ResetLevel();
    }

    void Update()
    {
        if (speed == 0) return;

        foreach (var road in currentRoads)
            road.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        if (currentRoads[0].transform.position.z < -10)
        {
            Destroy(currentRoads[0]);
            currentRoads.RemoveAt(0);
            speed = Math.Min(speed + 0.05f - (startSpeed - speed) * 0.01f, maxSpeed);
            CreateNextRoad();
        }
    }

    public void StartLevel()
    {
        speed = startSpeed;
        SwipeManager.instance.enabled = true;
    }

    private void CreateNextRoad()
    {
        Vector3 pos = new Vector3();
        if (currentRoads.Count > 0)
            pos = currentRoads[currentRoads.Count - 1].transform.position + new Vector3(0, 0, roadSegmentLength);
        GameObject roadSegment = Instantiate(
            roadPrefabs[UnityEngine.Random.Range(0, roadPrefabs.Count)],
            pos, Quaternion.identity, this.transform);
        currentRoads.Add(roadSegment);
    }

    public void ResetLevel()
    {
        speed = 0;
        while (currentRoads.Count > 0)
        {
            Destroy(currentRoads[0]);
            currentRoads.RemoveAt(0);
        }
        for (int i = 0; i < maxRoadCount; i++)
            CreateNextRoad();
        SwipeManager.instance.enabled = false;

        MapGenerator.instance.ResetMaps();
    }
}
