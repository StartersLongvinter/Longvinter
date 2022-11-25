using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestScript : MonoBehaviourPunCallbacks
{
    public string testPrefabName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 20 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(testPrefabName, new Vector3(0, 1, 0), Quaternion.identity);
        PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
    }
}
