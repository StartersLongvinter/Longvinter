using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public float moveSpeed;
    public Weapon weapon;
    public Transform leftHandIkTarget;
    public Transform rightHandIkTarget;
    public Vector3 AimLookPoint
    {
        get { return aimLookPoint; }
    }
    public bool IsAiming
    {
        get { return isAiming; }
    }

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private PlayerStat playerStat;

    private float horizontalAxis;
    private float verticalAxis;
    private Vector3 moveDirection;
    private Vector3 aimLookPoint;
    private float attackDelay;

    private bool doAttack;
    private bool isAttackReady;
    private bool isAiming;

    private float ikProgress;
    private float ikWeight;

    [SerializeField] GameObject chatInput;

    public bool testbool;

    #region Callback Methods
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerStat = GetComponent<PlayerStat>();

        if (photonView.IsMine)
        {
            Camera.main.GetComponent<CameraController>().Player = this.transform;
            Camera.main.GetComponent<CameraController>().PlayerController = this;
        }

        playerStat.ownerPlayerActorNumber = photonView.Owner.ActorNumber;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (!photonView.IsMine || chatInput.activeSelf) return;

        GetInput();
        Aim();
        Attack();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || chatInput.activeSelf) return;

        Move();
        Rotate();
    }

    private void OnAnimatorIK()
    {
        AnimateAim();
    }
    #endregion

    #region Public Methods
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((bool)isAiming);
        }
        else
        {
            isAiming = (bool)stream.ReceiveNext();
        }
    }
    #endregion

    #region Private Methods
    private void GetInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        isAiming = Input.GetButton("Fire2");
        photonView.RPC("SetIsAiming", RpcTarget.Others, isAiming);
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
            if (moveDirection == Vector3.zero) return;

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    private void Attack()
    {
        if (weapon == null) return;

        attackDelay += Time.deltaTime;
        isAttackReady = weapon.attackRate < attackDelay;

        if (isAiming && isAttackReady && doAttack)
        {
            if (weapon.type == Weapon.Type.Range)
            {
                weapon.Fire();
            }
            else if (weapon.type == Weapon.Type.Melee1)
            {
                weapon.Swing();

                playerAnimator.SetTrigger("doMeleeAttack");
            }

            attackDelay = 0;
        }
    }

    private void Aim()
    {
        if (weapon == null) return;

        if (isAiming && weapon.type == Weapon.Type.Melee1)
        {
            playerAnimator.SetBool("isMeleeAttackAim", true);
        }
        else
        {
            playerAnimator.SetBool("isMeleeAttackAim", false);
        }
    }

    private void AnimateAim()
    {
        float progressSpeed = Mathf.Lerp(1f, 3f, ikProgress);

        if (isAiming && weapon.type == Weapon.Type.Range || weapon.type == Weapon.Type.Melee2)
        {
            ikProgress = Mathf.Clamp(ikProgress + Time.deltaTime * progressSpeed, 0f, 1f);
        }
        else
        {
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

    [PunRPC]
    private void SetIsAiming(bool _isAiming)
    {
        isAiming = _isAiming;
    }
    #endregion
}
