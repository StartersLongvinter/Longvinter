using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

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
    private bool isPressedSpace;


    private float ikProgress;
    private float ikWeight;

    [SerializeField] GameObject chatInput;
    
    //낚시관련 변수
    [SerializeField] float maxInteractableDistance = 7;
    [SerializeField]
    bool isFishing;
    public bool eImageActivate;
    public bool isSuccessState;
    int eCount = 0;
    
    GameObject fish;
    
    private float timer = 0f;


    #region Callback Methods
    private void Awake()
    {
        this.gameObject.name = photonView.Owner.NickName;
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
        if (!photonView.IsMine || chatInput.activeSelf) return;

        Move();
        Rotate();
    }

    private void OnAnimatorIK()
    {
        AnimateAim();
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
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressedSpace = true;
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
                if (raycasthit.collider.GetComponent<FishingPoint>().isOccupied)
                {
                    Debug.Log("다른사람이 낚시중입니다.");
                }
                else
                {
                    Debug.Log("Fishing");
                    //낚시할때 fishingpoint를 바라보고 있어야 함
                    transform.LookAt(new Vector3(raycasthit.collider.transform.position.x , transform.position.y, raycasthit.collider.transform.position.z));
                    playerAnimator.SetTrigger("doFish");
                    fish = raycasthit.collider.GetComponent<FishingPoint>().SelectRandomFish();
                    StartCoroutine(CatchFish(raycasthit.collider.GetComponent<FishingPoint>()));
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
        if (isFishing)
        {
            return;
        }
        
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

    IEnumerator CatchFish(FishingPoint point)
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
            UIManager.instance.OpenSuccessImage(fish);
            point.IsFinished();
        }
        else
        {
            Debug.Log("Fail");
            point.IsFinished();
        }
        playerAnimator.SetBool("isFishing", false);
        yield return new WaitForSeconds(3);
        isSuccessState = false;
        isFishing = false;
    }
}
