using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Test_JJ : MonoBehaviourPunCallbacks
{
    [SerializeField] string prefabName = "Player_J";
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("TestJJ", new RoomOptions { MaxPlayers = 20 }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
    }
    void Update()
    {
    }
}
