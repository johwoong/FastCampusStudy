using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float distance = 3;

    void Start()
    {
        Managers.Input.DragAction -= CameraMove;
        Managers.Input.DragAction += CameraMove;

    }

    void Update()
    {

    }

    void CameraMove(float xmove, float ymove)
    {
        transform.rotation = Quaternion.Euler(ymove, xmove, 0);
        Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance);
        transform.position = player.transform.position - transform.rotation * reverseDistance;
    }
}