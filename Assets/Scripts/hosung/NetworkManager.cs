using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj;
                obj = GameObject.Find("NetworkManager");
                if (obj == null)
                {
                    obj = new GameObject("NetworkManager");
                    // obj.GetComponent<PhotonView>().TransferOwnership()
                    obj.AddComponent<PhotonView>();
                    instance = obj.AddComponent<NetworkManager>();
                }
                else
                {
                    obj.AddComponent<NetworkManager>();
                    instance = obj.GetComponent<NetworkManager>();
                }
            }
            return instance;
        }
    }
    public List<RoomInfo> rooms = new List<RoomInfo>();
    public string nickName = "";
    bool isLobby = true;
    public string playerPrefabName;

    Vector3 respawnPos = new Vector3(0, 0, 0);
    [SerializeField] GameObject roomPrefab;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Init(string _playerName, GameObject _roomPrefab)
    {
        if (PhotonNetwork.InLobby) return;

        playerPrefabName = _playerName;
        roomPrefab = _roomPrefab;
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

    public void OnClickCreate(int maxPlayer, bool isPVP, string password)
    {
        Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
        cp["maxPlayers"] = maxPlayer;
        cp["isPVP"] = isPVP;
        cp["password"] = password;
        PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
    }

    public bool OnClickJoinRoom(string roomName, string password)
    {
        //nickName = GameObject.Find("NickNameInput").GetComponent<TMP_InputField>().text;
        Debug.Log("checkInfo");
        if (!PhotonNetwork.InLobby) return false;
        foreach (RoomInfo room in rooms)
        {
            Debug.Log($"roomName : {roomName} / {(string)room.CustomProperties["roomName"]}");
            if (((string)room.CustomProperties["roomName"] == roomName) && ((int)room.CustomProperties["maxPlayers"] > (int)room.CustomProperties["curPlayer"]))
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
        if (!isLobby) CreateSinglePlayRoom(false, "");
    }

    void CreateSinglePlayRoom(bool isPVP, string password)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.CustomRoomProperties = new Hashtable() { { "maxPlayers", 1 }, { "roomName", nickName + "'s room" }, { "password", password }, { "curPlayer", 0 }, { "isPVP", isPVP } };
        // 방이름, 비밀번호 값을 로비에서도 받을 수 있도록 함 
        string[] newPropertiesForLobby = new string[5];
        newPropertiesForLobby[0] = "roomName";
        newPropertiesForLobby[1] = "password";
        newPropertiesForLobby[2] = "curPlayer";
        newPropertiesForLobby[3] = "isPVP";
        newPropertiesForLobby[4] = "maxPlayers";
        roomOptions.CustomRoomPropertiesForLobby = newPropertiesForLobby;
        PhotonNetwork.CreateRoom(nickName + "'s room", roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = nickName;
        //GameObject.Find("PasswordPanel").SetActive(false);
        var player = PhotonNetwork.Instantiate(playerPrefabName, respawnPos, Quaternion.identity);
        PhotonNetwork.Instantiate("HomeArea", new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f)), Quaternion.identity);

        int _actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("RenewalPlayerList", RpcTarget.All, _actorNumber, true);
        photonView.RPC("RenewalCurPlayers", RpcTarget.MasterClient, 1);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        photonView.RPC("RenewalPlayerList", RpcTarget.All, otherPlayer.ActorNumber, false);
        photonView.RPC("RenewalCurPlayers", RpcTarget.MasterClient, -1);
        photonView.RPC("RemovePlayerInList", RpcTarget.All, otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        isLobby = true;
        LoadLevel(0);
    }

    [PunRPC]
    void RenewalPlayerList(int playerActorNumber, bool isJoin = true)
    {
        if (isJoin)
        {
            PlayerList.Instance.players.Clear();
            PlayerList.Instance.players = PhotonNetwork.PlayerList.ToList();

            foreach (Player p in PlayerList.Instance.players)
                Debug.Log(p.NickName + " " + p.ActorNumber);
        }
        else
        {
            PlayerList.Instance.players.RemoveAll(x => x.ActorNumber == playerActorNumber);
            PlayerList.Instance.playerStats.RemoveAll(x => x.ownerPlayerActorNumber == playerActorNumber);
        }
    }

    [PunRPC]
    void RenewalCurPlayers(int curPlayerNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
            cp["curPlayer"] = (int)cp["curPlayer"] + curPlayerNumber;
            PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
            Debug.Log(cp["curPlayer"].ToString());
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RenewalRoomList(roomList);
    }

    public void RenewalRoomList(List<RoomInfo> roomList)
    {
        if (!isLobby) return;

        Transform roomBox = GameObject.Find("Content").transform;

        foreach (RoomInfo room in roomList)
        {
            if (rooms.Contains(room))
            {
                GameObject thisRoom = roomBox.Find((string)room.CustomProperties["roomName"]).gameObject;
                thisRoom.GetComponent<Room>().RoomInit((string)room.CustomProperties["roomName"], (int)room.CustomProperties["curPlayer"], (int)room.CustomProperties["maxPlayers"]);
                break;
            }
            if (room.RemovedFromList)
            {
                Destroy(roomBox.Find((string)room.CustomProperties["roomName"]));
                rooms.Remove(room);
                break;
            }

            rooms.Add(room);
            GameObject newRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            newRoom.transform.parent = roomBox;
            newRoom.GetComponent<Room>().RoomInit((string)room.CustomProperties["roomName"], (int)room.CustomProperties["curPlayer"], (int)room.CustomProperties["maxPlayers"]);
            newRoom.name = (string)room.CustomProperties["roomName"];
        }
    }

    // public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    // {
    // photonView.RPC("", RpcTarget.AllViaServer)
    // }

    [PunRPC]
    public void Kicked(int _actorNumber)
    {
        if (_actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void RemovePlayerInList(int _actorNumber)
    {
        PlayerList.Instance.playerStats.RemoveAll(x => x.ownerPlayerActorNumber == _actorNumber);
        PlayerList.Instance.players.RemoveAll(x => x.ActorNumber == _actorNumber);
        PlayerList.Instance.playerCharacters.RemoveAll(x => x.GetComponent<PlayerStat>().ownerPlayerActorNumber == _actorNumber);
        PlayerList.Instance.playersWithActorNumber.Remove(_actorNumber);
    }

    void Update()
    {

    }
}
