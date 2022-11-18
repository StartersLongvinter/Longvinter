using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerFishing : MonoBehaviour
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

    //���ð��� ����
    [SerializeField] float maxInteractableDistance = 7;
    //[SerializeField] GameObject pressEImage;
    //[SerializeField] GameObject successImage;
    //[SerializeField] TMP_Text fishName;
    [SerializeField] 
    bool isFishing;
    public bool eImageActivate;
    public bool isSuccessState;
    int eCount = 0;
    public Fish fish;
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
        Fishing();
        ECount();
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

    private void GetInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        isAiming = Input.GetButton("Fire2");
        doAttack = Input.GetButtonDown("Fire1");
    }

    private void Move()
    {
        if (!isFishing)
        {
            moveDirection = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
            float currentMoveSpeed = isAiming ? moveSpeed * 0.4f : moveSpeed;

            playerRigidbody.velocity = new Vector3(moveDirection.x * currentMoveSpeed, playerRigidbody.velocity.y, moveDirection.z * currentMoveSpeed);

            playerAnimator.SetBool("isWalking", moveDirection != Vector3.zero);

        }
    }

    private void Rotate()
    {
        if (isAiming && !isFishing)
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

    // ���� ������ �� �����ؾ� ��
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = attackRate < attackDelay;

        if (isAiming && isAttackReady && doAttack && !isFishing)
        {
            // �߻�

            attackDelay = 0;
        }
    }

    private void Aim()
    {
        // ī�޶� ���� ���� - ī�޶� ��Ʈ�ѷ����� ó��

    }

    private void AnimateAim()
    {
        if (!isFishing)
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
    }

    //raycast �̿� Ư�� object�� hit �Ǹ� fishing �Լ� ȣ��
    private void Fishing()
    {
        if (moveDirection != Vector3.zero || isFishing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //AI collider�� �ε����� üũ�� �ȵǴ� ���� �߻��� raycastall���� ����

        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100);

        foreach (var raycasthit in raycastHits)
        {
            float fishingDistance = Vector3.Distance(raycasthit.transform.position, transform.position);

            if (raycasthit.collider.gameObject.name == "FishingPoint" && fishingDistance < maxInteractableDistance && doAttack)
            {
                Debug.Log("Fishing");

                //�����Ҷ� fishingpoint�� �ٶ󺸰� �־�� ��
                transform.LookAt(raycasthit.collider.transform.position);

                playerAnimator.SetTrigger("doFish");
                fish = raycasthit.collider.GetComponent<Fish>();
                StartCoroutine(CatchFish());
            }
        }
    }

    void ECount()
    {
        if (eImageActivate)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                eCount++;
            }
        }
    }

    IEnumerator CatchFish()
    {
        isFishing = true;
        playerAnimator.SetBool("isFishing", true);
        eCount = 0;
        yield return new WaitForSeconds(Random.Range(5,11));
        eImageActivate = true;

        // E Ű�� ������� UI ������ �ؾ��� UIManager���� ����
        
        Debug.Log("Press E!");

        //2�� ������ �ڵ����� UI ��Ȱ��ȭ ���� �� ���� 10�� �Ѱ� Ŭ���ߴٸ� ����
        yield return new WaitForSeconds(2);

        eImageActivate = false;

        if (eCount >= 10)
        {
            isSuccessState = true;

            UIManager_BH.instance.OpenSuccessImage();
            
        }

        else Debug.Log("Fail");
      
        playerAnimator.SetBool("isFishing", false);

        yield return new WaitForSeconds(3);
        
        isSuccessState = false;
        isFishing = false;
        UIManager_BH.instance.OpenSuccessImage();
    }
}
