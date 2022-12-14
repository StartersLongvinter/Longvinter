using System;
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
    [SerializeField] Transform playerListPanel;

    private int selectedPlayerNumber;
    private Button playerKickBtn;


    private void Awake()
    {
        //playerKickBtn = playerListPrefab.transform.GetChild(1).GetComponent<Button>();
        
        //playerKickBtn.onClick.AddListener(KickPlayer);
    }

    public override void OnEnable()
    {
        InitInformation();
    }

    public void InitInformation()
    {
        while (playerListPanel.childCount > 0)
        {
            GameObject _listChild = playerListPanel.GetChild(0).gameObject;
            _listChild.transform.SetParent(null);
            Destroy(_listChild);
        }
        
        foreach (Player _player in PlayerList.Instance.players)
        {
            var playerList = Instantiate(playerListPrefab, playerListPrefab.transform.position, Quaternion.identity);

            if (PhotonNetwork.MasterClient == _player)
            {
                Debug.Log("방장임");
                playerList.transform.GetChild(1).gameObject.SetActive(false);
                playerList.transform.SetParent(playerListPanel, false);
                playerList.GetComponentInChildren<Text>().text = _player.NickName;
            }
            else
            {
                Debug.Log("유저임");
                playerList.transform.SetParent(playerListPanel, false);
                playerList.GetComponentInChildren<Text>().text = _player.NickName;
                playerList.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    if (!PhotonNetwork.IsMasterClient) return;
                    if (_player == PhotonNetwork.MasterClient) return;
                    //playerListPanel.gameObject.SetActive(true);
                    selectedPlayerNumber = _player.ActorNumber;
                    KickPlayer();
                });
            }
        }
    }

    public void UpdatePanel()
    {
        InitInformation();
    }

    public void KickPlayer()
    {
        NetworkManager.Instance.photonView.RPC("Kicked", RpcTarget.All, selectedPlayerNumber);
        InitInformation();
        UpdatePanel();
    }
}
