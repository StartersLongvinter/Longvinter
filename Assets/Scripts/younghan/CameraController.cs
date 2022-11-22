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
        Follow();
    }

    private void Follow()
    {
        if (player == null || playerController == null) return;

        Vector3 tempVector3 = new Vector3(transform.position.x - offset.x, player.position.y, transform.position.z - offset.z);
        float tempDistance = Vector3.Distance(player.position, tempVector3);

        if (playerController.IsAiming)
        {
            //smoothDampTime = Mathf.Lerp(0.2f, 0.1f, tempDistance / aimMaxDistance);

            float distance = Vector3.Distance(player.position, playerController.AimLookPoint);
            float clampDistance = Mathf.Clamp(distance, 0f, aimMaxDistance);

            Vector3 direction = (playerController.AimLookPoint - player.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);

            targetPosition = player.position + direction * clampDistance;
        }
        else
        {
            //smoothDampTime = Mathf.Lerp(0.07f, 0.2f, tempDistance / aimMaxDistance);

            targetPosition = player.position;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + offset, ref velocity, smoothDampTime);
    }
}
