using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Room : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI roomName;
    // [SerializeField] TMP_InputField roomPasswordInput;
    [SerializeField] Button roomJoinBtn;

    [SerializeField] TextMeshProUGUI countPlayer;

    public string password = "";
    public int maxPlayers = 1;
    public string RoomName = "";

    private string roomN = "";
    

    void Awake()
    {
        roomJoinBtn.onClick.AddListener(() =>
        {
            roomN = roomName.text;
            OnSelectRoom();
        });
    }

    public void OnSelectRoom()
    {
        GameObject canvas = GameObject.Find("Canvas");
        canvas.transform.Find("PasswordPopPanel").gameObject.SetActive(true);

        GameObject.Find("ConnectBtn").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ConnectBtn").GetComponent<Button>().onClick.AddListener(ClickEnterRoom);
    }

    public void ClickEnterRoom()
    {
        Debug.Log("submit");
        bool isConnect = NetworkManager.Instance.OnClickJoinRoom(roomN, GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text, maxPlayers, password, RoomName);

        if (!isConnect)
        {
            // 창이 흔들리는 효과
        }
    }

    public void RoomInit(string name, int curPlayers, int _maxPlayers, string _password)
    {
        roomName.text = name;
        maxPlayers = _maxPlayers;
        password = _password;
        countPlayer.text = $"{curPlayers}/{maxPlayers}";
    }

    void Update()
    {

    }
}
