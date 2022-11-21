using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player
    {
        set { player = value; }
    }
    public PlayerController PlayerController
    {
        set { playerController = value; }
    }

    [SerializeField] float aimMaxDistance;
    [SerializeField] float smoothDampTime;
    private Transform player;
    private PlayerController playerController;
    private Vector3 targetPosition;
    private Vector3 offset;
    private Vector3 velocity;

    void Start()
    {
        offset = transform.position;
    }

    void FixedUpdate()
    {
        if (player == null || playerController == null) return;

        if (playerController.IsAiming)
        {
            Vector3 direction = (playerController.AimLookPoint - player.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);

            float distance = Vector3.Distance(player.position, playerController.AimLookPoint);
            float clampDistance = Mathf.Clamp(distance, 0f, aimMaxDistance);

            targetPosition = player.position + direction * clampDistance;
        }
        else
        {
            targetPosition = player.position;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + offset, ref velocity, smoothDampTime);
    }
}
