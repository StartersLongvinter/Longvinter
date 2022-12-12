using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using UnityEngine.Android;
using System.IO;

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
    public bool isLobby = false;
    bool returnLobby = false;
    public string playerPrefabName;
    public string currentVersion = "";
    public string currentConnectionStatus;

    Vector3 respawnPos = new Vector3(0, 0, 0);
    [SerializeField] GameObject roomPrefab;
    bool isCheckedPermission = false;
    public List<string> badText = new List<string>();
    public SoundList backgroundMusics;

    void Awake()
    {
        currentVersion = Application.version;
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        if (NetworkManager.instance != null) Destroy(this.gameObject);
    }

    public void Init(string _playerName, GameObject _roomPrefab, SoundList _bgmList)
    {
        if (!isLobby)
        {
            playerPrefabName = _playerName;
            roomPrefab = _roomPrefab;
            backgroundMusics = _bgmList;
            DontDestroyOnLoad(this.gameObject);
            currentConnectionStatus = "서버에 연결중입니다";
            PhotonNetwork.ConnectUsingSettings();
        }
        isLobby = false;
    }

    void UpdateBadTexts()
    {
        Debug.Log(File.Exists(Application.streamingAssetsPath + "/BadWords.txt"));
        Debug.Log(Application.streamingAssetsPath + "/BadWords.txt");
        if (File.Exists(Application.streamingAssetsPath + "/BadWords.txt"))
        {
            badText = File.ReadAllText(Application.streamingAssetsPath + "/BadWords.txt", System.Text.Encoding.UTF8).Split("\n").ToList();
        }
    }

    public void SendWarningText(string message)
    {
        Text warningText = GameObject.Find("WarningText").GetComponent<Text>();
        if (warningText == null) return;
        warningText.text = message;
        warningText.GetComponent<WarningText>().isStart = true;
    }

    public void OnClickStart()
    {
        nickName = GameObject.Find("NickNameInput").transform.GetChild(0).GetChild(2).GetComponent<Text>().text;
        isLobby = false;

        PhotonNetwork.JoinLobby();
    }

    public void OnClickServer()
    {
        nickName = GameObject.Find("NickNameInput").transform.GetChild(0).GetChild(2).GetComponent<Text>().text;
        isLobby = true;
        PhotonNetwork.JoinLobby();
    }

    void LoadLevel(int i)
    {
        PhotonNetwork.LoadLevel(i);
    }

    public void OnClickCreate(string name, int maxPlayer, bool isPVP, string password)
    {
        rooms.RemoveAll(t => (string)t.CustomProperties["roomName"] == name + "'s room");

        Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
        cp["maxPlayers"] = maxPlayer;
        cp["isPVP"] = isPVP;
        cp["password"] = password;
        PhotonNetwork.CurrentRoom.SetCustomProperties(cp);

        rooms.Add(PhotonNetwork.CurrentRoom);
    }

    public bool OnClickJoinRoom(string roomName, string password, int _maxPlayers, string _realPassword)
    {
        //nickName = GameObject.Find("NickNameInput").GetComponent<TMP_InputField>().text;
        Debug.Log("checkInfo");
        if (!PhotonNetwork.InLobby) return false;
        foreach (RoomInfo room in rooms)
        {
            // Debug.Log($"roomName : {roomName} / {(string)room.CustomProperties["roomName"]}");
            // Debug.Log($"room Max : {(int)room.CustomProperties["maxPlayers"]}, cur : {(int)room.CustomProperties["curPlayer"]}");
            if (((string)room.CustomProperties["roomName"] == roomName) && (_maxPlayers > (int)room.CustomProperties["curPlayer"]))
            {
                Debug.Log($"roomName : {password} / {(string)room.CustomProperties["password"]}");
                if (currentVersion != (string)room.CustomProperties["version"])
                {
                    Debug.Log("[Error] version error!!");
                    SendWarningText("[Error] version error!!");
                    return false;
                }
                if (password == _realPassword)
                {
                    // PhotonNetwork.LocalPlayer.NickName = nickName;
                    SendWarningText($"Hello, {PhotonNetwork.LocalPlayer.NickName}!");
                    PhotonNetwork.LoadLevel(2);
                    PhotonNetwork.JoinRoom(roomName);
                    return true;
                }
                else break;
            }
        }
        Debug.Log("Can't enter " + roomName + " room");
        SendWarningText("[Error] Please check room info!");
        return false;
    }

    public override void OnConnectedToMaster()
    {
        UpdateBadTexts();
        currentConnectionStatus = "서버에 연결되었습니다";
        if (returnLobby)
        {
            isLobby = true;
            rooms.Clear();
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        returnLobby = false;
        Debug.Log("로비에 접속");
        LoadLevel(isLobby ? 1 : 2);
        if (!isLobby) CreateSinglePlayRoom(false, "");
    }

    void Renaming()
    {
        foreach (string _badText in badText)
        {
            if (nickName.Contains(_badText))
            {
                nickName = nickName.Replace(_badText, "nice");
                Debug.Log($"chat '{_badText}' is changed 'nice'");
            }
        }
    }

    void CreateSinglePlayRoom(bool isPVP, string password, int maxPlayers = 1, string roomName = "")
    {
        Renaming();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.CustomRoomProperties = new Hashtable() { { "maxPlayers", 1 }, { "roomName", roomName == "" ? nickName + "'s room" : roomName }, { "password", password }, { "curPlayer", 0 }, { "isPVP", isPVP }, { "version", currentVersion } };
        // 방이름, 비밀번호 값을 로비에서도 받을 수 있도록 함 
        string[] newPropertiesForLobby = new string[6];
        newPropertiesForLobby[0] = "roomName";
        newPropertiesForLobby[1] = "password";
        newPropertiesForLobby[2] = "curPlayer";
        newPropertiesForLobby[3] = "isPVP";
        newPropertiesForLobby[4] = "maxPlayers";
        newPropertiesForLobby[5] = "version";
        roomOptions.CustomRoomPropertiesForLobby = newPropertiesForLobby;

        PhotonNetwork.CreateRoom(roomName == "" ? nickName + "'s room" : roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        isLobby = false;

        Renaming();

        bool _sameName = false;

        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            if (nickName == _player.NickName && _player != PhotonNetwork.LocalPlayer)
            {
                _sameName = true;
                break;
            }
        }

        if (_sameName) PhotonNetwork.LocalPlayer.NickName = nickName + "2";
        else PhotonNetwork.LocalPlayer.NickName = nickName;
        //GameObject.Find("PasswordPanel").SetActive(false);
        var player = PhotonNetwork.Instantiate(playerPrefabName, respawnPos, Quaternion.identity);
        // PhotonNetwork.Instantiate("HomeArea", new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f)), Quaternion.identity);

        int _actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("RenewalPlayerList", RpcTarget.All, _actorNumber, true);
        photonView.RPC("RenewalCurPlayers", RpcTarget.MasterClient, 1);

        GameObject.Find("ShotdownBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            photonView.RPC("Kicked", RpcTarget.All, _actorNumber);
        });
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(GameObject.Find(otherPlayer.NickName + "HomeArea"));
        photonView.RPC("RenewalPlayerList", RpcTarget.All, otherPlayer.ActorNumber, false);
        photonView.RPC("RenewalCurPlayers", RpcTarget.MasterClient, -1);
        photonView.RPC("RemovePlayerInList", RpcTarget.All, otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        returnLobby = true;
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
            // Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
            // cp["curPlayer"] = (int)cp["curPlayer"] + curPlayerNumber;
            // PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
            // Debug.Log(cp["curPlayer"].ToString());
            Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
            cp["curPlayer"] = (int)cp["curPlayer"] + curPlayerNumber;
            PhotonNetwork.CurrentRoom.SetCustomProperties(cp);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count != 0 && roomList != null)
            RenewalRoomList(roomList);
    }

    public void RenewalRoomList(List<RoomInfo> roomList)
    {
        if (!isLobby || this.gameObject.scene.name == "LoginScene") return;

        Transform roomBox = GameObject.Find("Content").transform;

        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"{room.CustomProperties["roomName"]}, {room.CustomProperties["curPlayer"]}, {room.CustomProperties["maxPlayers"]}, {room.CustomProperties["password"]}");
            if (room.CustomProperties["roomName"] == null) break;
            if (room.RemovedFromList)
            {
                if (roomBox.Find((string)room.CustomProperties["roomName"]) != null)
                    Destroy(roomBox.Find((string)room.CustomProperties["roomName"]).gameObject);
                rooms.Remove(room);
                break;
            }
            if (rooms.Contains(room))
            {
                GameObject thisRoom = roomBox.Find((string)room.CustomProperties["roomName"]).gameObject;
                thisRoom.GetComponent<Room>().RoomInit((string)room.CustomProperties["roomName"], (int)room.CustomProperties["curPlayer"], (int)room.CustomProperties["maxPlayers"], (string)room.CustomProperties["password"]);
                break;
            }
            else if (!rooms.Contains(room))
            {
                rooms.Add(room);
                GameObject newRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
                newRoom.GetComponent<Room>().RoomInit((string)room.CustomProperties["roomName"], (int)room.CustomProperties["curPlayer"], (int)room.CustomProperties["maxPlayers"], (string)room.CustomProperties["password"] == null ? "" : (string)room.CustomProperties["password"]);
                newRoom.name = (string)room.CustomProperties["roomName"];
                newRoom.transform.parent = roomBox;
            }
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
