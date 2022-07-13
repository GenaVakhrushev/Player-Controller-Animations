using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset;

    public Transform Player;
    public float CameraSpeed;

    void Start()
    {
        offset = transform.position - Player.position;
    }

    void FixedUpdate()
    {
        Vector3 targetPos = Player.position + offset;
        Vector3 moveDir = targetPos - transform.position;
        transform.Translate(moveDir * CameraSpeed * Time.deltaTime, Space.World);
    }
}
