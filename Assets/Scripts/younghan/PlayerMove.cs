using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMove : MonoBehaviourPunCallbacks
{
    public float moveSpeed;
    public Transform leftHandIkTarget;
    public Transform rightHandIkTarget;

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private float horizontalAxis;
    private float verticalAxis;
    private Vector3 moveDirection;

    private float attackDelay;
    [SerializeField] private float attackRate = 0.5f;

    private bool doAttack;
    private bool isAttackReady;
    [SerializeField] private bool isAim; // �� Ȯ�ο� SerializeField, Ȯ���ϰ� SerializeField ������ ��

    private float ikProgress;
    private float ikWeight;

    private PlayerStat playerStat;

    #region Callback Methods
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        // 플레이어 스탯 스크립트 가져오고 플레이어 정보 저장 
        playerStat = GetComponent<PlayerStat>();
        playerStat.ownerPlayerActorNumber = photonView.Owner.ActorNumber;
    }

    public void SetOwnerPlayerInfo()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        GetInput();
        Aim();
        Attack();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
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
        isAim = Input.GetButton("Fire2");
        doAttack = Input.GetButtonDown("Fire1");
    }

    private void Move()
    {
        moveDirection = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        playerRigidbody.velocity = new Vector3(moveDirection.x * moveSpeed, playerRigidbody.velocity.y, moveDirection.z * moveSpeed);

        playerAnimator.SetBool("isWalk", moveDirection != Vector3.zero);
    }

    private void Rotate()
    {
        if (moveDirection == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    // ���� ������ �� �����ؾ� ��
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = attackRate < attackDelay;

        if (isAim && doAttack && isAttackReady)
        {
            // �߻�

            //playerAnimator.SetTrigger("doAttack");

            attackDelay = 0;
        }
    }

    private void Aim()
    {
        // ī�޶� ���� ����

        playerAnimator.SetBool("isAim", isAim);
    }

    private void AnimateAim()
    {
        if (isAim)
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
