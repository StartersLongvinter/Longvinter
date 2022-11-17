using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject chatBox;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] GameObject chatPrefab;
    [SerializeField] Transform playerPosition;

    void Awake()
    {
        this.transform.parent = null;
    }

    [PunRPC]
    public void SendNewMessage(string _message)
    {
        GameObject newChat = Instantiate(chatPrefab, chatPrefab.transform.position, Quaternion.Euler(0, 0, 0), chatBox.transform);
        newChat.GetComponentInChildren<TextMeshProUGUI>().text = _message;
        chatInput.gameObject.SetActive(false);
        chatInput.text = "";
    }

    void LateUpdate()
    {
        transform.position = playerPosition.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatInput.gameObject.activeSelf)
            {
                photonView.RPC("SendNewMessage", RpcTarget.All, chatInput.text);
            }
            else
            {
                chatInput.gameObject.SetActive(true);
                chatInput.Select();
            }
        }
    }
}
