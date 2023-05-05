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
    bool isJumping = false;
    float jumpPower = 15;
    float jumpGravity = -40;
    float realGravity = -9.8f;
    float laneOffset;

    void Start()
    {
        laneOffset = MapGenerator.instance.laneOffset;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;
        SwipeManager.instance.MoveEvent += MovePlayer;
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
        if (Input.GetKeyDown(KeyCode.W) && !isJumping)
        {
            Jump();
        }
    }

    void MovePlayer(bool[] swipes)
    {
        if (swipes[(int)SwipeManager.Direction.Left] && pointFinish > -laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Right] && pointFinish < laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Up] && !isJumping)
        {
            Jump();
        }
    }

    void Jump()
    {
        isJumping = true;
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());
    }

    IEnumerator StopJumpCoroutine()
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (rb.velocity.y != 0);
        isJumping = false;
        Physics.gravity = new Vector3(0, realGravity, 0);
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
