using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Title : MonoBehaviourPun
{
    public string playerPrefabName = "";
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject networkManagerObject;
    [SerializeField] private GameObject loadingPanel;

    void Awake()
    {
        if (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
            networkManagerObject.SetActive(true);
        NetworkManager.Instance.Init(playerPrefabName, roomPrefab);
    }

    void Update()
    {
        loadingPanel.GetComponentInChildren<Text>().text = NetworkManager.Instance.currentConnectionStatus;

        if (loadingPanel.GetComponentInChildren<Text>().text.Equals("서버에 연결되었습니다..."))
        {
            loadingPanel.SetActive(false);
        }
    }
}
