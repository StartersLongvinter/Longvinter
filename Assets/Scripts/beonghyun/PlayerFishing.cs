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

    //ï¿½ï¿½ï¿½Ã°ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
    [SerializeField] float maxInteractableDistance = 7;
<<<<<<< Updated upstream
    [SerializeField] GameObject pressEImage;
    [SerializeField] GameObject successImage;
    [SerializeField] TMP_Text fishName;
=======
    //[SerializeField] GameObject pressEImage;
    //[SerializeField] GameObject successImage;
    //[SerializeField] TMP_Text fishName;
    [SerializeField]
>>>>>>> Stashed changes
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

    // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Ø¾ï¿½ ï¿½ï¿½
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = attackRate < attackDelay;

        if (isAiming && isAttackReady && doAttack)
        {
            // ï¿½ß»ï¿½

            attackDelay = 0;
        }
    }

    private void Aim()
    {
        // Ä«ï¿½Þ¶ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ - Ä«ï¿½Þ¶ï¿½ ï¿½ï¿½Æ®ï¿½Ñ·ï¿½ï¿½ï¿½ï¿½ï¿½ Ã³ï¿½ï¿½

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

    //raycast ï¿½Ì¿ï¿½ Æ¯ï¿½ï¿½ objectï¿½ï¿½ hit ï¿½Ç¸ï¿½ fishing ï¿½Ô¼ï¿½ È£ï¿½ï¿½
    private void Fishing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //AI colliderï¿½ï¿½ ï¿½Îµï¿½ï¿½ï¿½ï¿½ï¿½ Ã¼Å©ï¿½ï¿½ ï¿½ÈµÇ´ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ß»ï¿½ï¿½ï¿½ raycastallï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½

        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100);

        foreach (var raycasthit in raycastHits)
        {
            float fishingDistance = Vector3.Distance(raycasthit.transform.position, transform.position);

            if (raycasthit.collider.gameObject.name == "FishingPoint" && fishingDistance < maxInteractableDistance && doAttack)
            {
                Debug.Log("Fishing");
<<<<<<< Updated upstream
=======

                //ï¿½ï¿½ï¿½ï¿½ï¿½Ò¶ï¿½ fishingpointï¿½ï¿½ ï¿½Ù¶óº¸°ï¿½ ï¿½Ö¾ï¿½ï¿½ ï¿½ï¿½
                transform.LookAt(raycasthit.collider.transform.position);

>>>>>>> Stashed changes
                playerAnimator.SetTrigger("doFish");
                fish = raycasthit.collider.GetComponent<Fish>();
                StartCoroutine(CatchFish());
            }

            //else Debug.Log("Can't Fish");
        }
        //if (Physics.Raycast(ray, out RaycastHit hit,100) && doAttack)
        //{
        //        //Æ¯Á¤ °Å¸® ³»¿¡¼­¸¸ ÀÛµ¿ÇÏ°Ô ÇØ¾ßÇÔ

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
<<<<<<< Updated upstream
        yield return new WaitForSeconds(Random.Range(5,11));

        // E Å°¸¦ ´©¸£¶ó´Â UI ³ª¿À°Ô ÇØ¾ßÇÔ
        pressEImage.SetActive(true);
=======
        yield return new WaitForSeconds(Random.Range(5, 11));
        eImageActivate = true;

        // E Å°ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ UI ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ø¾ï¿½ï¿½ï¿½ UIManagerï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½

>>>>>>> Stashed changes
        Debug.Log("Press E!");

        //2ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Úµï¿½ï¿½ï¿½ï¿½ï¿½ UI ï¿½ï¿½È°ï¿½ï¿½È­ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ 10ï¿½ï¿½ ï¿½Ñ°ï¿½ Å¬ï¿½ï¿½ï¿½ß´Ù¸ï¿½ ï¿½ï¿½ï¿½ï¿½
        yield return new WaitForSeconds(2);

        if (eCount >= 10)
        {
<<<<<<< Updated upstream
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
=======
            isSuccessState = true;

            // UIManager_BH.instance.OpenSuccessImage();

        }

        else Debug.Log("Fail");

        playerAnimator.SetBool("isFishing", false);

        yield return new WaitForSeconds(3);

        isSuccessState = false;
        isFishing = false;
        // UIManager_BH.instance.OpenSuccessImage();
>>>>>>> Stashed changes
    }
}
