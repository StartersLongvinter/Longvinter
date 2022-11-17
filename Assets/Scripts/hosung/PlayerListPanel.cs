using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerListPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerListPrefab;
    [SerializeField] TextMeshProUGUI playerCount;
    [SerializeField] Slider maxPlayerSlider;
    [SerializeField] Toggle PVPtoggle;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Transform playerlistBox;
    [SerializeField] GameObject kickPlayerPanel;
    [SerializeField] Button changeSettingButton;

    private int selectedPlayerNumber;

    public override void OnEnable()
    {
        InitInformation();
    }

    public void InitInformation()
    {
        while (playerlistBox.childCount > 0)
        {
            GameObject _listChild = playerlistBox.GetChild(0).gameObject;
            _listChild.transform.SetParent(null);
            Destroy(_listChild);
        }

        playerCount.text = $"{(int)PhotonNetwork.CurrentRoom.CustomProperties["curPlayer"]} / {(int)PhotonNetwork.CurrentRoom.CustomProperties["maxPlayers"]}";
        maxPlayerSlider.value = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxPlayers"];
        PVPtoggle.isOn = (bool)PhotonNetwork.CurrentRoom.CustomProperties["isPVP"];
        passwordInput.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["password"];

        maxPlayerSlider.interactable = (PhotonNetwork.IsMasterClient);
        PVPtoggle.interactable = (PhotonNetwork.IsMasterClient);
        passwordInput.interactable = (PhotonNetwork.IsMasterClient);
        changeSettingButton.interactable = (PhotonNetwork.IsMasterClient);

        foreach (Player _player in PlayerList.Instance.players)
        {
            var playerList = Instantiate(playerListPrefab, playerListPrefab.transform.position, Quaternion.identity, playerlistBox);
            playerList.GetComponentInChildren<Text>().text = _player.NickName;
            playerList.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!PhotonNetwork.IsMasterClient) return;
                if (_player == PhotonNetwork.MasterClient) return;
                kickPlayerPanel.SetActive(true);
                selectedPlayerNumber = _player.ActorNumber;
            });
        }
    }

    public void QuitKickPanel()
    {
        kickPlayerPanel.SetActive(false);
    }

    public void KickPlayer()
    {
        NetworkManager.Instance.photonView.RPC("Kicked", RpcTarget.All, selectedPlayerNumber);
        InitInformation();
        QuitKickPanel();
        this.gameObject.SetActive(false);
    }

    public void ChangeRoomInfo()
    {
        string password = passwordInput.text;
        int maxPlayercount = Mathf.RoundToInt(maxPlayerSlider.value);
        bool isPVP = PVPtoggle.isOn;
        NetworkManager.Instance.OnClickCreate(maxPlayercount, isPVP, password);

        this.gameObject.SetActive(false);
    }

    void Update()
    {

    }
}
