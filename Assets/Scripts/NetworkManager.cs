using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager Instance;
    public static NetworkManager instance
    {
        get { return Instance; }
    }
    private List<RoomInfo> rooms = new List<RoomInfo>();
    public string nickName = "";
    bool isLobby = true;

    [SerializeField] Vector3 respawnPos = new Vector3(0, 0, 0);
    [SerializeField] GameObject roomPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;

        DontDestroyOnLoad(this.gameObject);

        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickStart()
    {
        nickName = GameObject.Find("NickNameInput").GetComponent<TMP_InputField>().text;
        isLobby = false;

        PhotonNetwork.JoinLobby();
    }

    public void OnClickServer()
    {
        nickName = GameObject.Find("NickNameInput").GetComponent<TMP_InputField>().text;
        isLobby = true;
        PhotonNetwork.JoinLobby();
    }

    void LoadLevel(int i)
    {
        PhotonNetwork.LoadLevel(i);
    }

    public void OnClickCreate()
    {
        string password = GameObject.Find("PassWordInput").GetComponent<TMP_InputField>().text;
        if (nickName == "")
        {
            PhotonNetwork.Disconnect();
            return;
        }
        else
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            roomOptions.CustomRoomProperties = new Hashtable() { { "roomName", nickName + "'s room" }, { "password", password }, { "curPlayer", 0 } };
            // 방이름, 비밀번호 값을 로비에서도 받을 수 있도록 함 
            string[] newPropertiesForLobby = new string[3];
            newPropertiesForLobby[0] = "roomName";
            newPropertiesForLobby[1] = "password";
            newPropertiesForLobby[2] = "curPlayer";
            roomOptions.CustomRoomPropertiesForLobby = newPropertiesForLobby;
            PhotonNetwork.CreateRoom(nickName + "'s room", roomOptions);
        }
    }

    public bool OnClickJoinRoom(string roomName, string password)
    {
        //nickName = GameObject.Find("NickNameInput").GetComponent<TMP_InputField>().text;
        if (!PhotonNetwork.InLobby) return false;
        foreach (RoomInfo room in rooms)
        {
            Debug.Log($"roomName : {roomName} / {(string)room.CustomProperties["roomName"]}");
            if ((string)room.CustomProperties["roomName"] == roomName)
            {
                Debug.Log($"roomName : {password} / {(string)room.CustomProperties["password"]}");
                if (password == (string)room.CustomProperties["password"])
                {
                    // PhotonNetwork.LocalPlayer.NickName = nickName;
                    PhotonNetwork.LoadLevel(2);
                    PhotonNetwork.JoinRoom(roomName);
                    return true;
                }
                else break;
            }
        }
        Debug.Log("Can't enter " + roomName + " room");
        return false;
    }

    public override void OnConnectedToMaster()
    {

    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속");
        LoadLevel(isLobby ? 1 : 2);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = nickName;
        GameObject.Find("PasswordPanel").SetActive(false);
        var player = PhotonNetwork.Instantiate("Player", respawnPos, Quaternion.identity);

        photonView.RPC("RenewalCurPlayers", RpcTarget.MasterClient, 1);
    }

    public override void OnLeftRoom()
    {
        // onePlayerleft? 이걸로 해야지 함수가 실행됨 
        photonView.RPC("RenewalCurPlayers", RpcTarget.MasterClient, -1);
    }

    [PunRPC]
    void RenewalCurPlayers(int i)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
            cp["curPlayer"] = (int)cp["curPlayer"] + i;
            PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
            Debug.Log(cp["curPlayer"].ToString());
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!isLobby) return;

        Transform roomBox = GameObject.Find("Content").transform;

        foreach (RoomInfo room in roomList)
        {
            if (rooms.Contains(room)) return;
            if (room.RemovedFromList)
            {
                Destroy(roomBox.Find((string)room.CustomProperties["roomName"]));
                rooms.Remove(room);
                return;
            }

            rooms.Add(room);
            GameObject newRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            newRoom.transform.parent = roomBox;
            newRoom.GetComponent<Room>().RoomInit((string)room.CustomProperties["roomName"], (int)room.CustomProperties["curPlayer"], (int)room.MaxPlayers);
            newRoom.name = (string)room.CustomProperties["roomName"];
        }
    }

    void Update()
    {

    }
}
