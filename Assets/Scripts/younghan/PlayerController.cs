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
    public Transform cameraFollowTarget;
    [HideInInspector] public bool isBuilding = false;

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;
    private PlayerStat playerStat;
    private CameraController cameraController;

    private float horizontalAxis;
    private float verticalAxis;
    [HideInInspector] public Vector3 moveDirection;
    private Vector3 aimLookPoint;
    private float attackDelay;

    private bool doAttack;
    private bool doGreet;
    public bool isAttackReady;
    private bool isAiming;
    private bool isPressedSpace;
    private bool isGreeting;

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
    IEnumerator fishingCoroutine;
    FishingPoint currentFishingPoint;

    private bool isAuto;
    private bool isReadyToSaw = false;

    #region Callback Methods
    private void Awake()
    {
        this.gameObject.name = photonView.Owner.NickName;
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerStat = GetComponent<PlayerStat>();
        playerStat.ownerPlayerActorNumber = photonView.Owner.ActorNumber;

        if (photonView.IsMine)
        {
            cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.player = this.transform;
            cameraController.playerController = this;
            foreach (var virtualCamera in cameraController.virtualCameras)
                virtualCamera.Follow = cameraFollowTarget;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine || chatInput.activeSelf) return;

        GetInput();
        Rotate();
        AnimateOneHandAim();
        if (weaponData != null)
            photonView.RPC("SwitchWeaponPosition", RpcTarget.All, weaponData.eqIndex);
        Attack();
        Greet();
        Fishing();
        ECount();
        ChangeTurretMode();

        if (isPressedSpace)
            timer += Time.deltaTime;

        if (timer > 0.3f)
        {
            timer = 0f;
            isPressedSpace = false;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || chatInput.activeSelf)
            return;

        Move();
    }

    private void OnAnimatorIK()
    {
        AnimateTwoHandAim();
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
                    SoundManager.Instance.PlayToolSound("InventorySounds", 4);
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

        //deadbag하고 상호작용
        else if (other.tag.Equals("DeadBag"))
        {
            if (isPressedSpace)
            {
                isPressedSpace = false;
                UIManager.instance.OpenDeadBagInventory();
                //GetComponent<PlayerInventory>().AddItem(other.gameObject);
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
                    if (hit.collider.tag.Equals("DeadBag"))
                    {
                        UIManager.instance.OpenDeadBagInventory();
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
    [PunRPC]
    public void SwitchWeaponPosition(int eqIndex)
    {
        if (isAiming || isFishing)
        {
            bagEquipPoint.GetChild(eqIndex).gameObject.SetActive(false);
            handEquipPoint.GetChild(eqIndex).gameObject.SetActive(true);
        }
        else
        {
            bagEquipPoint.GetChild(eqIndex).gameObject.SetActive(true);
            handEquipPoint.GetChild(eqIndex).gameObject.SetActive(false);
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
            if (Input.GetButtonDown("Fire2"))
                attackDelay = weaponData.wpAttackRate * 0.5f;

            isAiming = Input.GetButton("Fire2");
            if (!isAiming && isReadyToSaw)
            {
                isReadyToSaw = false;
                SoundManager.Instance.StopSound(1);
            }
        }

        doAttack = Input.GetButtonDown("Fire1");
        doGreet = Input.GetKeyDown(KeyCode.C);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressedSpace = true;
        }
    }
    private void Move()
    {
        moveDirection = new Vector3(horizontalAxis, 0, verticalAxis).normalized;
        float currentMoveSpeed = isAiming ? playerStat.moveSpeed * 0.4f : playerStat.moveSpeed;
        playerRigidbody.velocity = new Vector3(moveDirection.x * currentMoveSpeed, playerRigidbody.velocity.y, moveDirection.z * currentMoveSpeed);
        playerAnimator.SetBool("isWalking", moveDirection != Vector3.zero);

        if (moveDirection != Vector3.zero && isFishing)
        {
            CancelFish();
        }

        if (moveDirection != Vector3.zero)
        {
            SoundManager.Instance.PlayPlayerSound("GrassStepSounds", -1, true, true);
            playerStat.ChangeStatus((int)PlayerStat.Status.Walk);
        }
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
        if (weaponData == null)
            return;

        attackDelay += Time.deltaTime;
        isAttackReady = weaponData.wpAttackRate < attackDelay;

        if (isAiming && isAttackReady)
        {
            Weapon weapon = handEquipPoint.GetChild(weaponData.eqIndex).gameObject.GetComponent<Weapon>();

            if (doAttack)
            {
                if (weaponData.eqClassify == EquipmentData.EquipmentClassify.RangeWeapon)
                {
                    weapon.Fire();

                    if (weaponData.name == "SemiAutomatic" || weaponData.name == "AssaultRifle") SoundManager.Instance.PlayToolSound("RifleSounds", -1);
                    else SoundManager.Instance.PlayToolSound("ShotSounds", -1);

                    StartCoroutine(cameraController.Shake(3f, 15f, 0.05f));

                    attackDelay = 0;
                }
                else if (weaponData.eqClassify == EquipmentData.EquipmentClassify.MeleeWeapon && weaponData.eqPosition == EquipmentData.EquipmentPosition.OneHand)
                {
                    weapon.Slash();

                    playerAnimator.SetTrigger("doSlash");

                    attackDelay = 0;
                }
            }
            else
            {
                if ((weaponData.eqClassify == EquipmentData.EquipmentClassify.MeleeWeapon && weaponData.eqPosition == EquipmentData.EquipmentPosition.TwoHand))
                {
                    weapon.Saw();

                    if (!isReadyToSaw)
                    {
                        SoundManager.Instance.PlayToolSound("ChainsawSounds", 0, true, true);
                        isReadyToSaw = true;
                    }
                    else
                        SoundManager.Instance.PlayToolSound("ChainsawSounds", 1, false);

                    StartCoroutine(cameraController.Shake(3f, 15f, 0.05f));

                    attackDelay = 0;
                }
            }
        }
    }

    private void AnimateOneHandAim()
    {
        if (weaponData == null)
            return;

        if (isAiming && weaponData.eqClassify == EquipmentData.EquipmentClassify.MeleeWeapon && weaponData.eqPosition == EquipmentData.EquipmentPosition.OneHand)
            playerAnimator.SetBool("isOneHandAim", true);
        else
            playerAnimator.SetBool("isOneHandAim", false);
    }

    private void AnimateTwoHandAim()
    {
        if (weaponData == null)// | isFishing)
            return;

        float progressSpeed = Mathf.Lerp(1f, 10f, ikProgress);

        if (weaponData.eqPosition == EquipmentData.EquipmentPosition.TwoHand && (isAiming || isFishing))
            ikProgress = Mathf.Clamp(ikProgress + Time.deltaTime * progressSpeed, 0f, 1f);
        else
            ikProgress = Mathf.Clamp(ikProgress - Time.deltaTime * progressSpeed, 0f, 0.5f);

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

    private void Greet()
    {
        if (doGreet && !isGreeting)
        {
            if (photonView.IsMine && !chatInput.activeSelf)
            {
                chatInput.GetComponentInParent<ChatManager>().ActivateNickname(1.7f / 2f);
                playerAnimator.SetTrigger("doGreet");

                isGreeting = true;
                Invoke("SetFalseIsGreeting", 1.7f / 2f);
            }
        }
    }

    private void SetFalseIsGreeting()
    {
        isGreeting = false;
    }

    [PunRPC]
    private void SetIsAiming(bool _isAiming)
    {
        isAiming = _isAiming;
    }

    [PunRPC]
    private void SetWeaponData(bool isNull = true, int _index = 0)
    {
        Debug.Log("Set " + _index);
        if (isNull)
            weaponData = null;
        else
            weaponData = weaponDatas[_index];
    }

    [PunRPC]
    private void ActiveOffEquipment(int _index)
    {
        if (_index == -1) bagEquipPoint.transform.GetChild(weaponData.eqIndex).gameObject.SetActive(false);
        else bagEquipPoint.transform.GetChild(_index).gameObject.SetActive(false);
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
            if (doAttack && raycasthit.collider.gameObject.name.Contains("Turret") && Distance < maxInteractableDistance &&
                raycasthit.collider.gameObject.GetComponent<PhotonView>().Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                isAuto = !raycasthit.collider.gameObject.GetComponent<TurretController>().IsAuto;
                turret.IsAuto = isAuto;
                turret.ChangeTurretModeColor();
                return;
            }
        }
    }

    //raycast 이용 특정 object와 hit 되면 fishing 함수 호출
    private void Fishing()
    {
        if (weaponData == null)
            return;

        if (moveDirection != Vector3.zero || isFishing || isAiming
            || (weaponData.eqClassify != EquipmentData.EquipmentClassify.Default || weaponData.eqPosition != EquipmentData.EquipmentPosition.TwoHand))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //AI collider와 부딪혀서 체크가 안되는 현상 발생함 raycastall으로 검출
        RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100);
        foreach (var raycasthit in raycastHits)
        {
            float fishingDistance = Vector3.Distance(raycasthit.transform.position, transform.position);
            if (raycasthit.collider.gameObject.name == "FishingPoint" && fishingDistance < maxInteractableDistance && doAttack)
            {
                currentFishingPoint = raycasthit.collider.GetComponent<FishingPoint>();

                if (currentFishingPoint.isWait)
                {
                    NotificationManager.instance.WarningNotification();
                    Debug.Log("Fish are surprised by sudden movement. Cannot use fishingpoint");
                    return;
                }
                Debug.Log("Fishing");

                //낚시할때 fishingpoint를 바라보고 있어야 함
                transform.LookAt(new Vector3(raycasthit.collider.transform.position.x, transform.position.y, raycasthit.collider.transform.position.z));
                //playerAnimator.SetTrigger("doFish");
                fish = raycasthit.collider.GetComponent<FishingPoint>().SelectRandomFish();

                fishingCoroutine = CatchFish(raycasthit.collider.GetComponent<FishingPoint>());
                //StartCoroutine(CatchFish(raycasthit.collider.GetComponent<FishingPoint>()));
                StartCoroutine(fishingCoroutine);

                SoundManager.Instance.PlayToolSound("FishingSounds", 0);
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

    void CancelFish()
    {
        playerAnimator.SetTrigger("cancelFish");
        isFishing = false;
        //playerAnimator.SetBool("isFishing", false);
        currentFishingPoint.WaitPoint();
        StopCoroutine(fishingCoroutine);
    }

    IEnumerator CatchFish(FishingPoint point)
    {
        isFishing = true;
        //playerAnimator.SetBool("isFishing", true);
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
            UIManager.instance.OpenSuccessImage(fish);
            point.IsFinished();
            SoundManager.Instance.PlayToolSound("FishingSounds", Random.Range(1, 3));
        }
        else
        {
            Debug.Log("Fail");
            point.IsFinished();
            SoundManager.Instance.PlayToolSound("FishingSounds", 0);
        }
        isFishing = false;
        //playerAnimator.SetBool("isFishing", false);
    }
    #endregion
}
