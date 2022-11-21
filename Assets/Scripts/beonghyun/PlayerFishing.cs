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
    [SerializeField] GameObject pressEImage;
    [SerializeField] GameObject successImage;
    [SerializeField] TMP_Text fishName;
    bool isFishing;
    int eCount = 0;
    Fish fish;
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

    // ���� ������ �� �����ؾ� ��
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = attackRate < attackDelay;

        if (isAiming && isAttackReady && doAttack)
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

    //raycast �̿� Ư�� object�� hit �Ǹ� fishing �Լ� ȣ��
    private void Fishing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //AI collider�� �ε����� üũ�� �ȵǴ� ���� �߻��� raycastall���� ����

        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100);

        foreach (var raycasthit in raycastHits)
        {
            float fishingDistance = Vector3.Distance(raycasthit.transform.position, transform.position);

            if (raycasthit.collider.gameObject.name == "FishingPoint" && fishingDistance < maxInteractableDistance && doAttack)
            {
                Debug.Log("Fishing");
                playerAnimator.SetTrigger("doFish");
                fish = raycasthit.collider.GetComponent<Fish>();
                StartCoroutine(CatchFish());
            }

            //else Debug.Log("Can't Fish");
        }
        //if (Physics.Raycast(ray, out RaycastHit hit,100) && doAttack)
        //{
        //        //Ư�� �Ÿ� �������� �۵��ϰ� �ؾ���

        //    float fishingDistance = Vector3.Distance(hit.transform.position, transform.position);

        //    if (hit.collider.gameObject.name == "FishingPoint" && fishingDistance < maxInteractableDistance)
        //    {
        //        Debug.Log("Fishing");
        //        playerAnimator.SetTrigger("doFish");
        //        fish = hit.collider.GetComponent<Fish>();
        //        StartCoroutine(CatchFish());
        //    }

        //    else Debug.Log("Can't Fish");
        // } 

    }

    void ECount()
    {
        if (isFishing)
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
        playerAnimator.SetBool("isFishing", isFishing);
        eCount = 0;
        yield return new WaitForSeconds(Random.Range(5, 11));

        // E Ű�� ������� UI ������ �ؾ���
        pressEImage.SetActive(true);
        Debug.Log("Press E!");

        //2�� ������ �ڵ����� UI ��Ȱ��ȭ ���� �� ���� 10�� �Ѱ� Ŭ���ߴٸ� ����
        yield return new WaitForSeconds(2);

        if (eCount >= 10)
        {
            int fishNumber = Random.Range(0, fish.fishList.Length);
            Debug.Log("Success! Caught " + fish.fishList[fishNumber]);
            fishName.text = fish.fishList[fishNumber];
            successImage.SetActive(true);
        }

        else Debug.Log("Fail");

        pressEImage.SetActive(false);

        isFishing = false;
        playerAnimator.SetBool("isFishing", isFishing);

        yield return new WaitForSeconds(3);
        successImage.SetActive(false);
    }
}