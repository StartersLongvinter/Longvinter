using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Transform leftHandIkTarget;
    public Transform rightHandIkTarget;

    private PlayerStat playerStat;

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private float horizontalAxis;
    private float verticalAxis;
    private Vector3 moveDirection;


    private float attackDelay;
    [SerializeField] private float attackRate = 0.5f;

    private bool doAttack;
    private bool isAttackReady;
    public bool isAiming;

    private float ikProgress;
    private float ikWeight;

    public Vector3 aimLookPoint;

    #region Callback Methods
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        playerStat = GetComponent<PlayerStat>();
        //playerStat.ownerPlayerActorNumber = photonView.Owner.ActorNumber;
    }

    private void Start()
    {

    }

    private void Update()
    {
        //if (!photonView.IsMine) return;

        GetInput();
        Aim();
        Attack();
    }

    private void FixedUpdate()
    {
        //if (!photonView.IsMine) return;

        Move();
        Rotate();
    }

    private void OnAnimatorIK()
    {
        AnimateAim();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
    private void GetInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        isAiming = Input.GetButton("Fire2");
        doAttack = Input.GetButtonDown("Fire1");
    }

    private void Move()
    {
        moveDirection = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
        float currentMoveSpeed = isAiming ? moveSpeed * 0.4f : moveSpeed;

        playerRigidbody.velocity = new Vector3(moveDirection.x * currentMoveSpeed, playerRigidbody.velocity.y, moveDirection.z * currentMoveSpeed);

        playerAnimator.SetBool("isWalking", moveDirection != Vector3.zero);
    }

    private void Rotate()
    {
        if (isAiming)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (plane.Raycast(cameraRay, out rayDistance))
            {
                aimLookPoint = cameraRay.GetPoint(rayDistance);

                transform.LookAt(new Vector3(aimLookPoint.x, transform.position.y, aimLookPoint.z));
            }
        }
        else
        {
            if (moveDirection == Vector3.zero)
                return;

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        
    }

    // 근접 무기일 때 생각해야 함
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = attackRate < attackDelay;

        if (isAiming && isAttackReady && doAttack)
        {
            // 발사

            attackDelay = 0;
        }
    }

    private void Aim()
    {
        // 카메라 시점 변경 - 카메라 컨트롤러에서 처리

    }

    private void AnimateAim()
    {
        if (isAiming)
        {
            float progressSpeed = (ikProgress < 0.3f) ? 1f : 2f;
            ikProgress = Mathf.Clamp(ikProgress + Time.deltaTime * progressSpeed, 0f, 1f);
        }
        else
        {
            float progressSpeed = (ikProgress < 0.3f) ? 1f : 2f;
            ikProgress = Mathf.Clamp(ikProgress - Time.deltaTime * progressSpeed, 0f, 0.5f);
        }

        ikWeight = Mathf.Lerp(0f, 1f, ikProgress);

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);

        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIkTarget.position);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIkTarget.position);

        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIkTarget.rotation);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIkTarget.rotation);
    }
    #endregion
}
