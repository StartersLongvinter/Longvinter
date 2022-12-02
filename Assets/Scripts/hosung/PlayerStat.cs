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

    public bool isCold = false;
    public bool inWater = false;
    public float autoDamageValue = 1f;
    [SerializeField] float damagedTime = 6f;
    [SerializeField] float timePercentInSnowField = 0.3f;
    [SerializeField] float timePercentWhileWalk = 0.8f;
    [SerializeField] float timePercentInWater = 0.5f;
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
        if (photonView.IsMine)
        {
            localPlayer = this;
            photonView.RPC("AddPlayerStatAndCharacter", RpcTarget.AllBuffered);
        }
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
        if (!photonView.IsMine) return;

        startTime += Time.deltaTime;

        float _targetTime = damagedTime;
        if (isCold) _targetTime *= timePercentInSnowField;
        if (inWater) _targetTime *= timePercentInWater;
        if (status == Status.walk) _targetTime *= timePercentWhileWalk;

        if (startTime >= _targetTime)
        {
            photonView.RPC("AddHp", RpcTarget.AllViaServer, -1f * autoDamageValue);
            startTime = 0f;
        }

        if (hp < 30)
        {
            currentHPImage.color = warningColor;
        }
        else
            currentHPImage.color = normalColor;
    }

    void Update()
    {
        float _hpValue = hp / 90f;
        currentHPImage.fillAmount = _hpValue;
    }

    // Public Methods
    // ����

    [PunRPC]
    public void AddHp(float _hp)
    {
        hp += _hp;
        if (hp < 0)
        {
            hp = 0;
            status = Status.die;
        }
    }

    // ����
    public void AddMoney(int _money)
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
        }
        else
        {
            hp = (float)stream.ReceiveNext();
            maxHp = (float)stream.ReceiveNext();
            money = (int)stream.ReceiveNext();
            status = (Status)(int)stream.ReceiveNext();
        }
    }
}
