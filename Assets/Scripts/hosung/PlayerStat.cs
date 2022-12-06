using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStat : MonoBehaviourPunCallbacks, IPunObservable
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
        idle = 1,
        walk,
        attack,
        damaged,
        die
    }
    public Status status;

    public float hp;
    public float maxHp = 100f;
    public int money;

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

    public float coldReductionPercentage = 0;                           // 추위 감소 효과 퍼센트
    public float armorPercentage = 0;                                   // 아머 효과 퍼센트
    public float increaseSpeedPercentage = 0;                           // 스피드 증가 효과 퍼센트
    public float fishingPercentage = 0;                                 // 낚시 효과 퍼센트

    float startTime = 0f;
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color warningColor;
    private Image currentHPImage;
    public Animator currentHPAnimator;

    // Callback Methods
    void Awake()
    {
        currentHPImage = GameObject.Find("HPvalue").GetComponent<Image>();
        currentHPAnimator = GameObject.Find("MaskImage").GetComponent<Animator>();
        hp = maxHp;
        if (photonView.IsMine && LocalPlayer == null)
        {
            localPlayer = this;
            photonView.RPC("AddPlayerStatAndCharacter", RpcTarget.AllBuffered);
            JsonManager.Instance.LoadDate();
        }

    }

    public void ChangeStatus(int _index)
    {
        status = (Status)_index;
    }

    public void GetEffect(ItemData.ItemEffect effectType, float percent)
    {

    }

    public void InitEffect(ItemData.ItemEffect effectType)
    {

    }

    [PunRPC]
    void AddPlayerStatAndCharacter()
    {
        PlayerList.Instance.playerStats.Add(this);
        PlayerList.Instance.playerCharacters.Add(this.gameObject);
        PlayerList.Instance.playersWithActorNumber.Add(photonView.Owner.ActorNumber, this.gameObject);
        status = Status.idle;

        foreach (GameObject player in PlayerList.Instance.playersWithActorNumber.Values)
        {
            Debug.Log(player.GetPhotonView().Owner.NickName);
        }
    }

    void LoseStamina()
    {
        if (!photonView.IsMine || status == Status.die) return;

        startTime += Time.deltaTime;

        float _targetDamage = autoDamageValue;
        if (isCold)
        {
            if (status == Status.walk) _targetDamage = autoDamageValue * damagePercentWhileWalkInSnow;
            else _targetDamage = autoDamageValue * damagePercentInSnowField;
        }

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
            currentHPImage.color = warningColor;
        }
        else
            currentHPImage.color = normalColor;
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

    // Public Methods
    // ����

    [PunRPC]
    public void ChangeHp(float _hp)
    {
        if (hp + _hp > 100)
        {
            hp = maxHp;
        }
        else
        {
            hp += _hp;
        }

        if (hp < 0)
        {
            hp = 0;
            ChangeStatus((int)Status.die);
            this.gameObject.layer = 8;
        }
    }

    // ����
    public void ChangeMoney(int _money)
    {
        money += _money;
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
}
