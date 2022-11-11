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

        GameObject.Find("ConnectBtn").GetComponent<Button>().onClick.AddListener(ClickEnterRoom);
    }

    public void ClickEnterRoom() 
    {
        NetworkManager.instance.OnClickJoinRoom(roomN, GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text);
    }

    public void RoomInit(string name)
    {
        roomName.text = name;
    }

    void Update()
    {

    }
}
