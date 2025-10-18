using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightMovement : MonoBehaviour
{

    public GameObject[] PathNode;
    public GameObject Player;
    public float MoveSpeed;
    float Timer;
    static Vector3 CurrentPositionHolder;
    int CurrentNode;
    private Vector2 startPosition;
    private Vector3 originPosition;

    public Transform nodes1;
    public Transform nodes2;

    void Awake()
    {
        startPosition = transform.position;
        if (Player != null)
        {
            originPosition = Player.transform.position;
        }
    }
    void Start()
    {
        int randomIndex = Random.Range(0, 2);
        //Debug.Log(randomIndex);
        if (nodes1 == null && nodes2 == null)
        {

            GameObject stationaryNode = new GameObject("stationaryNode");
            stationaryNode.transform.position = startPosition;
            //create one node at the location
            PathNode = new GameObject[1];
            PathNode[0] = stationaryNode;
        }
        else if (nodes1 == null) {
            PathNode = new GameObject[nodes2.childCount];
            for (int m = 0; m < nodes2.childCount; m++)
            {
                PathNode[m] = nodes2.GetChild(m).gameObject;
            }
        }
        else if (nodes2 == null)
        {
            PathNode = new GameObject[nodes1.childCount];
            for (int n = 0; n < nodes1.childCount; n++)
            {
                PathNode[n] = nodes1.GetChild(n).gameObject;
            }
        }
        else
        {
            if (randomIndex == 0)
            {
                PathNode = new GameObject[nodes1.childCount];
                for (int i = 0; i < nodes1.childCount; i++)
                {
                    PathNode[i] = nodes1.GetChild(i).gameObject;
                }
            }
            else
            {
                PathNode = new GameObject[nodes2.childCount];
                for (int j = 0; j < nodes2.childCount; j++)
                {
                    PathNode[j] = nodes2.GetChild(j).gameObject;
                }

            }
            CheckNode();
        }
    }

    void CheckNode()
    {
        if (CurrentNode >=PathNode.Length)
        {
            return;
        }
        Timer = 0;
        startPosition = Player.transform.position;
        CurrentPositionHolder = PathNode[CurrentNode].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckNode();
        Timer += Time.deltaTime * MoveSpeed;

        if (Player.transform.position != CurrentPositionHolder)
        {
            Player.transform.position = Vector3.MoveTowards(Player.transform.position,CurrentPositionHolder, MoveSpeed * Time.deltaTime);
        }
        else
        {

            if (CurrentNode < PathNode.Length-1)
            {
                CurrentNode++;
                CheckNode();
            }
            else {
                CurrentNode = 0;
                CheckNode();
            
            }
        }
    }
    public void OnReset() {
        if (Player != null)
        {
            Player.transform.position = originPosition;
        }
        Timer = 0;
        PathNode = null;
        CurrentNode = 0;
        Start();
    }
}