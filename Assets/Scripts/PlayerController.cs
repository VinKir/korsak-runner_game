using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    Vector3 startGamePosition;
    Quaternion startGameRotation;
    float pointStart;
    float pointFinish;
    float laneChangeSpeed = 15f;
    bool isMoving = false;
    float lastVectorX;
    Coroutine movingCoroutine;

    public float laneOffset = 1.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && pointFinish > -laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.D) && pointFinish < laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
    }


    void MoveHorizontal(float speed)
    {
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffset;

        if (isMoving) { StopCoroutine(movingCoroutine); isMoving = false; }
        movingCoroutine = StartCoroutine(MoveCoroutine(speed));
    }

    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate();

            rb.velocity = new Vector3(vectorX, rb.velocity.y, 0);
            lastVectorX = vectorX;
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        isMoving = false;
    }

    public void StartGame()
    {
        RoadGenerator.instance.StartLevel();
    }

    public void ResetGame()
    {
        rb.velocity = Vector3.zero;
        pointFinish = 0;
        pointStart = 0;
        RoadGenerator.instance.ResetLevel();
        transform.position = startGamePosition;
        transform.rotation = startGameRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Lose")
            ResetGame();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "NotLose")
        {
            MoveHorizontal(-lastVectorX);
        }
    }
}
