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

    [SerializeField] Vector3 offset;

    private Camera mainCamera;
    private Camera uiCamera;

    void Awake()
    {
        this.gameObject.name = photonView.Owner.NickName + " Canvas";
        this.transform.parent = null;
        mainCamera = Camera.main;
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        this.GetComponent<Canvas>().worldCamera = uiCamera;
    }

    [PunRPC]
    public void SendNewMessage(string _message)
    {
        GameObject newChat = Instantiate(chatPrefab, chatBox.transform.position, Quaternion.Euler(0, 0, 0));
        newChat.transform.SetParent(chatBox.transform, false);
        // newChat.transform.position = new Vector3(newChat.transform.position.x, newChat.transform.position.y, 0);
        newChat.GetComponentInChildren<TextMeshProUGUI>().text = _message;
        chatInput.gameObject.SetActive(false);
        chatInput.text = "";
    }

    void LateUpdate()
    {

    }

    void Update()
    {
        Vector3 _finalPosition = mainCamera.WorldToScreenPoint(playerPosition.position);
        _finalPosition = uiCamera.ScreenToWorldPoint(_finalPosition);
        _finalPosition = new Vector3(_finalPosition.x, _finalPosition.y, 0);
        chatBox.transform.position = _finalPosition + offset;
        chatInput.transform.position = _finalPosition + offset;

        if (Input.GetKeyDown(KeyCode.Return) && photonView.IsMine)
        {
            if (chatInput.gameObject.activeSelf)
            {
                photonView.RPC("SendNewMessage", RpcTarget.All, chatInput.text);
            }
            else
            {
                chatInput.gameObject.SetActive(true);
            }
            chatInput.ActivateInputField();
            chatInput.Select();
        }
    }
}
