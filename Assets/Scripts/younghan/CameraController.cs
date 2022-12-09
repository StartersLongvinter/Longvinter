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

    [SerializeField] float maxDistance;
    [SerializeField] float smoothDampTime;
    [SerializeField] float scrollSpeed = 2000f;

    private Transform player;
    private PlayerController playerController;
    private Vector3 targetPosition;
    private Vector3 offset;
    private Vector3 velocity;

    public Vector3 originalPositionOffset;
    public Vector3 targetPositionOffset;
    public Vector3 originalRotationOffset;
    public Vector3 targetRotationOffset;

    public Vector2 yOffsetMinMax;
    public Vector2 zOffsetMinMax;


    void Start()
    {
        offset = transform.position;
    }

    void Update()
    {
        Zoom();
    }

    void FixedUpdate()
    {
        Follow();
    }

    private void RotateTopView()
    {

    }



    private void Zoom()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        Vector3 cameraDirection = transform.localRotation * Vector3.forward;

        offset += cameraDirection * scrollWheel * scrollSpeed * Time.deltaTime;
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
            float clampDistance = Mathf.Clamp(distance, 0f, maxDistance);

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
