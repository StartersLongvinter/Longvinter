using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Vector3 AimLookPoint { get { return aimLookPoint; } }
    public bool IsAiming { get { return isAiming; } }
    public EquipmentData weaponData;
    public EquipmentData[] weaponDatas;
    public Transform leftHandIkTarget;
    public Transform rightHandIkTarget;
    public Transform bagEquipPoint;
    public Transform handEquipPoint;
    [HideInInspector] public bool isBuilding = false;

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
    private float timer = 0f;

    private GameObject fish;
    [SerializeField] float maxInteractableDistance = 7;
    private bool isFishing;
    private bool isSuccessState;
    public bool eImageActivate;
    private int eCount = 0;

    private bool isAuto;

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

    [PunRPC]
    private void SetWeaponData(bool isNull = true, int _index = 0)
    {
        Debug.Log("Set " + _index);
        if (isNull)
            weaponData = null;
        else
        {
            weaponData = weaponDatas[_index];
        }
    }

    [PunRPC]
    private void ActiveOffEquipment(int _index)
    {
        if (_index == -1) bagEquipPoint.transform.GetChild(weaponData.eqIndex).gameObject.SetActive(false);
        else bagEquipPoint.transform.GetChild(_index).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!photonView.IsMine || chatInput.activeSelf) return;

        GetInput();
        Aim();
        Attack();
        Fishing();
        ECount();
        ChangeTurretMode();

        if (isPressedSpace)
        {
            timer += Time.deltaTime;
        }

        if (timer > 0.3f)
        {
            timer = 0f;
            isPressedSpace = false;
        }

        if (weaponData != null) photonView.RPC("SwitchWeaponPosition", RpcTarget.All, weaponData.eqIndex);
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

    #region Public Methods
    // 이거 합쳐야 함
    [PunRPC]
    public void SwitchWeaponPosition(int _emIndex)
    {
        if (isAiming)
        {
            bagEquipPoint.GetChild(_emIndex).gameObject.SetActive(false);
            handEquipPoint.GetChild(_emIndex).gameObject.SetActive(true);
        }
        else
        {
            bagEquipPoint.GetChild(_emIndex).gameObject.SetActive(true);
            handEquipPoint.GetChild(_emIndex).gameObject.SetActive(false);
        }
    }
    #endregion

    #region Private Methods
    private void GetInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        photonView.RPC("SetIsAiming", RpcTarget.Others, isAiming);

        if (weaponData != null && !isBuilding)
        {
            isAiming = Input.GetButton("Fire2");
        }

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
                Debug.Log("Fishing");
                //낚시할때 fishingpoint를 바라보고 있어야 함
                transform.LookAt(new Vector3(raycasthit.collider.transform.position.x, transform.position.y, raycasthit.collider.transform.position.z));
                playerAnimator.SetTrigger("doFish");
                fish = raycasthit.collider.GetComponent<FishingPoint>().SelectRandomFish();
                StartCoroutine(CatchFish(raycasthit.collider.GetComponent<FishingPoint>()));
            }
        }
    }

    private void ChangeTurretMode()
    {
        if (isAiming)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100);
        foreach (var raycasthit in raycastHits)
        {
            if (raycasthit.collider.gameObject.GetComponent<TurretController>() == null)
                continue;
            TurretController turret = raycasthit.collider.gameObject.GetComponent<TurretController>();
            if (turret.turretOwner == "")
                return;
            float Distance = Vector3.Distance(raycasthit.transform.position, transform.position);
            if (doAttack &&raycasthit.collider.gameObject.name.Contains("Turret") && Distance < maxInteractableDistance && 
                raycasthit.collider.gameObject.GetComponent<PhotonView>().Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                isAuto = !raycasthit.collider.gameObject.GetComponent<TurretController>().IsAuto;
                turret.IsAuto = isAuto;
                turret.ChangeTurretModeColor();
                return;
            }
        }
    }



    private void ECount()
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
            float currentMoveSpeed = isAiming ? playerStat.moveSpeed * 0.4f : playerStat.moveSpeed;
            playerRigidbody.velocity = new Vector3(moveDirection.x * currentMoveSpeed, playerRigidbody.velocity.y, moveDirection.z * currentMoveSpeed);
            playerAnimator.SetBool("isWalking", moveDirection != Vector3.zero);
        }

        if (moveDirection != Vector3.zero)
            playerStat.ChangeStatus((int)PlayerStat.Status.Walk);
        else
            playerStat.ChangeStatus((int)PlayerStat.Status.Idle);
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
        if (weaponData == null) return;

        Weapon weapon = handEquipPoint.GetChild(weaponData.eqIndex).gameObject.GetComponent<Weapon>();

        attackDelay += Time.deltaTime;
        isAttackReady = weapon.attackRate < attackDelay;

        if (isAiming && isAttackReady && doAttack)
        {
            if (weapon.type == Weapon.Type.OneHandRange || weapon.type == Weapon.Type.TwoHandRange)
            {
                weapon.Fire();
            }
            else if (weapon.type == Weapon.Type.OneHandMelee)
            {
                weapon.Swing();

                playerAnimator.SetTrigger("doMeleeAttack");
            }

            attackDelay = 0;
        }
    }

    private void Aim()
    {
        if (weaponData == null) return;

        if (isAiming && weaponData.eqClassify == EquipmentData.EquipmentClassify.MeleeWeapon && weaponData.eqPosition == EquipmentData.EquipmentPosition.OneHand)
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
        if (weaponData == null | isFishing) return;

        float progressSpeed = Mathf.Lerp(1f, 10f, ikProgress);
        
        if (isAiming && weaponData.eqPosition == EquipmentData.EquipmentPosition.TwoHand)
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

    IEnumerator CatchFish(FishingPoint point)
    {
        isFishing = true;
        playerAnimator.SetBool("isFishing", true);
        eCount = 0;
        yield return new WaitForSeconds(Random.Range(playerStat.fishingSpeed, playerStat.fishingSpeed * 2));
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
    #endregion
}
