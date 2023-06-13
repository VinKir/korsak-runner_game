using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Rootobject
{
    public Class1[] Property1;
}

[System.Serializable]
public class Class1
{
    public string Question;
    public Answer[] Answers;
    public int RightAnswerNumber;
}

[System.Serializable]
public class Answer
{
    public string answer;
}

public class MapGenerator : MonoBehaviour
{
    public int maxSpaceBeforeQuestions = 7;
    int currentSpaceBeforeQuestions = 7;
    public GameObject questionGO;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI questionAnswer1;
    public TextMeshProUGUI questionAnswer2;
    public TextMeshProUGUI questionAnswer3;
    int rightWay = 0;


    public int itemSpace = 10;
    public int itemCountInMap = 5;
    int mapSize;
    enum TrackPos { Left = -1, Center = 0, Right = 1 }

    public List<GameObject> ObstaclesTopPrefab;
    public List<GameObject> ObstaclesBottomPrefab;
    public List<GameObject> ObstaclesFullPrefab;
    public GameObject ObstacleRightAnswerPrefab;

    public float laneOffset = 1.5f;
    public List<GameObject> maps = new List<GameObject>();
    public List<GameObject> activeMaps = new List<GameObject>();

    public static MapGenerator instance;

    private void Awake()
    {
        instance = this;

        mapSize = itemCountInMap * itemSpace;
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        foreach (var map in maps)
            map.SetActive(false);
    }

    void Start()
    {
        currentSpaceBeforeQuestions = maxSpaceBeforeQuestions;
    }

    void Update()
    {
        if (RoadGenerator.instance.speed == 0) return;

        foreach (var map in activeMaps)
            map.transform.position -= new Vector3(0, 0, RoadGenerator.instance.speed * Time.deltaTime);
        if (activeMaps[0].transform.position.z < -mapSize)
        {
            currentSpaceBeforeQuestions--;
            RemoveFirstActiveMap();
            AddActiveMap();
        }
    }

    public void ResetMaps()
    {
        currentSpaceBeforeQuestions = maxSpaceBeforeQuestions;
        while (activeMaps.Count > 0)
            RemoveFirstActiveMap();
        AddActiveMap();
        AddActiveMap();
    }

    void RemoveFirstActiveMap()
    {
        if (activeMaps[0].name == "QuestionMap")
        {
            activeMaps.RemoveAt(0);
            Destroy(transform.Find("QuestionMap").gameObject);
            return;
        }

        activeMaps[0].SetActive(false);
        maps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    void AddActiveMap()
    {
        int r = Random.Range(0, maps.Count);
        GameObject go = maps[r];
        if (currentSpaceBeforeQuestions <= 0)
        {
            StartCoroutine("AskQuestion");
            go = MakeQuestionMap();
            currentSpaceBeforeQuestions = maxSpaceBeforeQuestions;
        }
        else
        {
            maps.RemoveAt(r);
        }
        go.SetActive(true);
        foreach (Transform child in go.transform)
            child.gameObject.SetActive(true);
        go.transform.position = activeMaps.Count > 0 ?
                                activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * mapSize :
                                new Vector3(0, 0, 10);
        activeMaps.Add(go);
    }

    IEnumerator AskQuestion()
    {
        questionGO.SetActive(true);
        Time.timeScale = 0.3f;

        var path = Application.streamingAssetsPath + "/" + "questions.json";
        var jsonFile = JsonUtility.FromJson<Rootobject>("{\"Property1\":" + File.ReadAllText(path) + "}");
        var quest = jsonFile.Property1[Random.Range(0, jsonFile.Property1.Length)];
        questionText.text = quest.Question;
        questionAnswer1.text = quest.Answers[0].answer;
        questionAnswer2.text = quest.Answers[1].answer;
        questionAnswer3.text = quest.Answers[2].answer;
        Debug.Log(quest.RightAnswerNumber);
        rightWay = quest.RightAnswerNumber;

        yield return new WaitForSeconds(10 * 0.3f);
        Time.timeScale = 1.0f;
        questionGO.SetActive(false);
    }

    private GameObject MakeQuestionMap()
    {
        GameObject result = new GameObject("QuestionMap");
        result.transform.parent = this.transform;
        for (int i = 0; i < itemCountInMap; i++)
        {
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;

            if (i == itemCountInMap - 1)
                for (int j = -1; j < 2; j++)
                {
                    if (j + 1 == rightWay)
                        obstacle = ObstacleRightAnswerPrefab;
                    else
                        obstacle = ObstaclesFullPrefab[0];
                    trackPos = (TrackPos)j;
                    Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
                    if (obstacle != null)
                        Instantiate(obstacle, obstaclePos, Quaternion.identity, result.transform);
                }
        }
        return result;
    }

    private GameObject MakeMap1()
    {
        GameObject result = new GameObject("Map1");
        result.transform.parent = this.transform;
        for (int i = 0; i < itemCountInMap; i++)
        {
            GameObject obstacle;
            TrackPos trackPos;
            var tracks = new List<TrackPos>() { TrackPos.Left, TrackPos.Center, TrackPos.Right };
            for (int j = 0; j < Random.Range(1, 3); j++)
            {
                if (true)
                {
                    trackPos = tracks[Random.Range(0, tracks.Count)];
                    tracks.Remove(trackPos);
                    obstacle = RandomObstacle();
                }

                Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
                if (obstacle != null)
                    Instantiate(obstacle, obstaclePos, Quaternion.identity, result.transform);
            }
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
