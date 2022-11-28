using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Title : MonoBehaviourPun
{
    public string playerPrefabName = "";
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject networkManagerObject;

    void Awake()
    {
        if (!PhotonNetwork.InLobby)
            networkManagerObject.SetActive(true);
        NetworkManager.Instance.Init(playerPrefabName, roomPrefab);
    }

    void Update()
    {

    }
}
