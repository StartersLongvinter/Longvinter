using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

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
    private bool isPressedSpace;
    public bool isAiming;
    private float ikProgress;
    private float ikWeight;
    public Vector3 aimLookPoint;
    //낚시관련 변수
    [SerializeField] float maxInteractableDistance = 7;
    //[SerializeField] GameObject pressEImage;
    //[SerializeField] GameObject successImage;
    //[SerializeField] TMP_Text fishName;
    [SerializeField]
    bool isFishing;
    public bool eImageActivate;
    public bool isSuccessState;
    int eCount = 0;
    public ItemData[] fish;


    private float timer = 0f;
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

        if (isPressedSpace)
        {
            timer += Time.deltaTime;
        }

        if (timer > 0.3f)
        {
            timer = 0f;
            isPressedSpace = false;
        }
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
        //doAttack = Input.GetButtonDown("Fire1");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressedSpace = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            doAttack = true;
        }
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
    // 근접 무기일 때 생각해야 함
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        isAttackReady = attackRate < attackDelay;
        if (isAiming && isAttackReady && doAttack && !isFishing)
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
    //raycast 이용 특정 object와 hit 되면 fishing 함수 호출
    private void Fishing()
    {
        if (moveDirection != Vector3.zero || isFishing) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //AI collider와 부딪혀서 체크가 안되는 현상 발생함 raycastall으로 검출
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100);
        foreach (var raycasthit in raycastHits)
        {
            float fishingDistance = Vector3.Distance(raycasthit.transform.position, transform.position);
            if (raycasthit.collider.gameObject.name == "FishingPoint" && fishingDistance < maxInteractableDistance && doAttack)
            {
                Debug.Log("Fishing");
                //낚시할때 fishingpoint를 바라보고 있어야 함
                transform.LookAt(raycasthit.collider.transform.position);
                playerAnimator.SetTrigger("doFish");
                fish = raycasthit.collider.GetComponent<Fish>().fishList;
                StartCoroutine(CatchFish());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Item"))
        {
            if (isPressedSpace)
            {
                isPressedSpace = false;
                if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM - 1)
                {
                    Debug.Log(other.gameObject.name);
                    GetComponent<Encyclopedia>().itemData = other.GetComponent<Item>().item;
                    GetComponent<Encyclopedia>().GainItem();
                    GetComponent<PlayerInventory>().AddItem(other.gameObject);
                    //GetComponent<PlayerInventory>().itemList.Add(other.gameObject);
                }
                else
                {
                    Debug.Log("인벤토리 가득참");
                }
            }
            else if (doAttack)
            {
                doAttack = false;
                RaycastHit[] hits;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                hits = Physics.RaycastAll(ray);
                
                var distinctHits = hits.DistinctBy(x => x.collider.name);
                
                foreach (var hit in distinctHits)
                {
                    if (hit.collider.tag.Equals("Item"))
                    {
                        if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM - 1)
                        {
                            Debug.Log(other.gameObject.name);
                            GetComponent<Encyclopedia>().itemData = other.GetComponent<Item>().item;
                            GetComponent<Encyclopedia>().GainItem();
                            GetComponent<PlayerInventory>().AddItem(other.gameObject);
                        }
                        else
                        {
                            Debug.Log("인벤토리 가득참");
                        }
                    }
                }
            }
        }
        else if (other.tag.Equals("Equipment"))
        {
            if (isPressedSpace)
            {
                isPressedSpace = false;
                if (GetComponent<PlayerInventory>().inventoryCount <= GetComponent<PlayerInventory>().MAXITEM - 1)
                {
                    GetComponent<PlayerInventory>().AddItem(other.gameObject);
                }
                else
                {
                    Debug.Log("인벤토리 가득참");
                }
            }

            if (doAttack)
            {
                doAttack = false;
                RaycastHit[] hits;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                hits = Physics.RaycastAll(ray);

                var distinctHits = hits.DistinctBy(x => x.collider.name);

                foreach (var hit in distinctHits)
                {
                    if (hit.collider.tag.Equals("Equipment"))
                    {
                        if (GetComponent<PlayerInventory>().inventoryCount <= GetComponent<PlayerInventory>().MAXITEM - 1)
                        {
                            GetComponent<PlayerInventory>().AddItem(other.gameObject);
                        }
                        else
                        {
                            Debug.Log("인벤토리 가득참");
                        }
                    }
                }
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
        yield return new WaitForSeconds(Random.Range(5, 11));
        eImageActivate = true;
        // E 키를 누르라는 UI 나오게 해야함 UIManager에서 실행
        Debug.Log("Press E!");
        //2초 지나면 자동으로 UI 비활성화 만약 그 사이 10번 넘게 클릭했다면 성공
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
    }
}