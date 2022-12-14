using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStat : MonoBehaviourPunCallbacks, IPunObservable, IDamageable
{
    private static PlayerStat localPlayer;
    public static PlayerStat LocalPlayer
    {
        get { return localPlayer; }
    }

    // 나중에 플레이어 actornumber로 플레이어 판별 등을 할 예정 
    [Header("OwnerPlayerInfo")]
    public int ownerPlayerActorNumber;

    public enum Status
    {
        Idle = 1,
        Walk,
        Attack,
        Damaged,
        Die,
        Aim
    }
    public Status status;

    public float hp;
    public float maxHp = 100f;
    public int money;
    [SerializeField] private float originalSpeed;
    public float moveSpeed;
    [SerializeField] private float originalfishingSpeed = 5;
    public float fishingSpeed;

    public bool isFight = false;
    public bool isCold = false;
    public bool inWater = false;

    // 데미지 입는 시간 간격 
    [SerializeField] float damagedTime = 1f;

    // 상황에 맞게 데미지 설정
    [SerializeField] public float autoDamageValue = 0.15f;              // Defalut : 초원에서 걸어다니는 경우 
    [SerializeField] float damagePercentInSnowField = 3.3f;             // 설원에서 서있는 경우
    [SerializeField] float damagePercentWhileWalkInSnow = 5.5f;         // 설원에서 걸어다니는 경우
    [SerializeField] float damagePercentInWater = 13.3f;                // 깊은 물 속에 있는 경우 

    float coldReductionPercentage = 0;                           // 추위 감소 효과 퍼센트
    float armorPercentage = 0;                                   // 아머 효과 퍼센트
    float increaseSpeedPercentage = 0;                           // 스피드 증가 효과 퍼센트
    float fishingPercentage = 0;                                 // 낚시 효과 퍼센트

    float startTime = 0f;
    [SerializeField] private Color hpNormalColor;
    [SerializeField] private Color hpWarningColor;
    private Image currentHPImage;
    public Animator currentHPAnimator;

    public static Color playerOriginalColor;
    private SkinnedMeshRenderer meshRenderer;
    public bool isColorChanged = false;

    // EquipmentData 변수 추가
    EquipmentData currentWeapon;

    void Awake()
    {
        moveSpeed = originalSpeed;
        fishingSpeed = originalfishingSpeed;
        currentHPImage = GameObject.Find("HPvalue").GetComponent<Image>();
        currentHPAnimator = GameObject.Find("MaskImage").GetComponent<Animator>();
        hp = maxHp;
        if (photonView.IsMine && LocalPlayer == null)
        {
            localPlayer = this;
            photonView.RPC("AddPlayerStatAndCharacter", RpcTarget.AllBuffered);

            JsonManager.Instance.LoadPlayerData();
            if (PhotonNetwork.IsMasterClient) JsonManager.Instance.LoadRoomData();
        }
    }

    void Update()
    {
        LoseStamina();

        if (photonView.IsMine)
        {
            float _hpValue = hp / maxHp;
            currentHPImage.fillAmount = _hpValue;
        }
    }

    public void GetEffect(ItemData.ItemEffect effectType, float percent)
    {
        switch (effectType)
        {
            case (ItemData.ItemEffect.ColdDamageReduction):
                coldReductionPercentage = percent;
                break;
            case (ItemData.ItemEffect.GetHardAmor):
                armorPercentage = percent;
                break;
            case (ItemData.ItemEffect.IncreaseMovementSpeed):
                increaseSpeedPercentage = percent;
                moveSpeed += originalSpeed * increaseSpeedPercentage;
                break;
            case (ItemData.ItemEffect.IncreaseFishingSpeed):
                fishingPercentage = percent;
                fishingSpeed -= originalfishingSpeed * fishingPercentage;
                break;
        }
    }

    public void InitEffect(ItemData.ItemEffect effectType)
    {
        switch (effectType)
        {
            case (ItemData.ItemEffect.ColdDamageReduction):
                coldReductionPercentage = 0;
                break;
            case (ItemData.ItemEffect.GetHardAmor):
                armorPercentage = 0;
                break;
            case (ItemData.ItemEffect.IncreaseMovementSpeed):
                increaseSpeedPercentage = 0;
                moveSpeed = originalSpeed;
                break;
            case (ItemData.ItemEffect.IncreaseFishingSpeed):
                fishingPercentage = 0;
                fishingSpeed = originalfishingSpeed;
                break;
        }
    }

    [PunRPC]
    void AddPlayerStatAndCharacter()
    {
        PlayerList.Instance.playerStats.Add(this);
        PlayerList.Instance.playerCharacters.Add(this.gameObject);
        PlayerList.Instance.playersWithActorNumber.Add(photonView.Owner.ActorNumber, this.gameObject);
        status = Status.Idle;

        foreach (GameObject player in PlayerList.Instance.playersWithActorNumber.Values)
        {
            Debug.Log(player.GetPhotonView().Owner.NickName);
        }
    }

    void LoseStamina()
    {
        if (!photonView.IsMine || status == Status.Die) return;

        startTime += Time.deltaTime;

        float _targetDamage = autoDamageValue;
        if (isCold)
        {
            if (status == Status.Walk) _targetDamage = autoDamageValue * damagePercentWhileWalkInSnow;
            else _targetDamage = autoDamageValue * damagePercentInSnowField;
        }

        _targetDamage -= _targetDamage * coldReductionPercentage;

        if (inWater) _targetDamage = autoDamageValue * damagePercentInWater;

        if (startTime >= damagedTime)
        {
            if (hp <= (maxHp / 9f) && !isCold && !inWater)
            {
                startTime = 0f;
            }
            else
            {
                photonView.RPC("ChangeHp", RpcTarget.AllViaServer, -1f * _targetDamage);
                startTime = 0f;
            }
        }

        if (hp <= (maxHp / 9f) * 3f)
        {
            currentHPImage.color = hpWarningColor;
        }
        else
            currentHPImage.color = hpNormalColor;
    }


    [PunRPC]
    public void ApplyDamage(float damage)
    {
        ChangeHp(-1f * damage);
        ChangePlayersColor();
    }

    public void ChangeStatus(int _index)
    {
        if (_index == (int)Status.Die) return;

        status = (Status)_index;
    }

    [PunRPC]
    public void ChangeHp(float _hp)
    {
        if (status == Status.Damaged && _hp < 0)
        {
            _hp -= _hp * armorPercentage;
        }

        if (hp + _hp > 100)
        {
            hp = maxHp;
        }

        else
        {
            hp += _hp;
        }

        if (hp <= 0)
        {
            Debug.Log("Dead");
            hp = 0;
            ChangeStatus((int)Status.Die);
            this.gameObject.layer = 8;
            if (currentWeapon == null)
                return;
            DropItem();
        }
    }

    public void ChangeMoney(int _money)
    {
        money += _money;
    }

    [PunRPC]
    public void ChangePlayersColor()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (photonView.IsMine)
        {
            Debug.Log(photonView.Owner.NickName);
            currentHPAnimator.SetTrigger("isDamaged");
            //ApplyDamage(damage);
            ChangeStatus((int)Status.Damaged);
        }
        StopCoroutine(ResetColor());
        StartCoroutine(ResetColor());
    }

    IEnumerator ResetColor()
    {
        isColorChanged = true;
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.08f);
        meshRenderer.material.color = Color.white;
        isColorChanged = false;
        ChangeStatus((int)Status.Idle);
    }

    // 플레이어 자원 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(maxHp);
            stream.SendNext(money);
            stream.SendNext((int)status);
            stream.SendNext((bool)isFight);
            stream.SendNext((bool)isCold);
            stream.SendNext((bool)inWater);
        }

        else
        {
            hp = (float)stream.ReceiveNext();
            maxHp = (float)stream.ReceiveNext();
            money = (int)stream.ReceiveNext();
            status = (Status)(int)stream.ReceiveNext();
            isFight = (bool)stream.ReceiveNext();
            isCold = (bool)stream.ReceiveNext();
            inWater = (bool)stream.ReceiveNext();
        }
    }

    public void DropItem()
    {
        currentWeapon = this.gameObject.GetComponent<PlayerController>().weaponData;
        PhotonNetwork.Instantiate("ItemPrefabs/" + currentWeapon.name, this.gameObject.
            transform.position + new Vector3(Random.Range(-1, 1f), 0.5f, Random.Range(-1, 1f)), Quaternion.identity);
    }
}
