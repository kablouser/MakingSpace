using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform cameraHolder;

    [SerializeField]
    private float smoothSpeed = 0.2f;
    public Vector3 offset;

    void FixedUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(cameraHolder.position, desiredPosition, smoothSpeed);
        cameraHolder.position = smoothedPosition;
    }
}
