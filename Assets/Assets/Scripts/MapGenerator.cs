using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    int itemSpace = 15;
    int itemCountInMap = 5;
    enum TrackPos { Left = -1, Center = 0, Right = 1 }

    public float laneOffset = 1.5f;
    public List<GameObject> maps = new List<GameObject>();

    public static MapGenerator instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        maps.Add(MakeMap1());
    }

    void Update()
    {

    }

    private GameObject MakeMap1()
    {
        throw new NotImplementedException();
    }
}
