using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject chatBox;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] GameObject chatPrefab;
    GameObject nicknamePrefab;
    [SerializeField] Transform playerPosition;

    [SerializeField] Vector3 offset;

    private Camera mainCamera;
    private Camera uiCamera;
    GameObject nicknameText;
    GameObject chattingSign;

    void Awake()
    {
        nicknamePrefab = Resources.Load("Nickname") as GameObject;

        this.gameObject.name = photonView.Owner.NickName + " Canvas";
        this.transform.parent = null;
        mainCamera = Camera.main;
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        this.GetComponent<Canvas>().worldCamera = uiCamera;
        this.transform.position = GameObject.Find("Canvas").transform.position;
    }

    // Activate nickname object when you express emotions.
    public void ActivateNickname(float activeTime)
    {
        photonView.RPC("SetActiveNickname", RpcTarget.All, true);
        StartCoroutine(DisappearNickname(activeTime));
    }

    [PunRPC]
    void SetActiveNickname(bool isActive)
    {
        nicknameText.SetActive(isActive);
    }

    IEnumerator DisappearNickname(float activeTime)
    {
        yield return new WaitForSeconds(activeTime);
        photonView.RPC("SetActiveNickname", RpcTarget.All, false);
    }

    [PunRPC]
    public void SendNewMessage(string _message)
    {
        if (string.IsNullOrEmpty(_message))
        {
            chatInput.gameObject.SetActive(false);
            chatInput.text = "";
            return;
        }

        GameObject newChat = Instantiate(chatPrefab, chatBox.transform.position, Quaternion.Euler(0, 0, 0));
        newChat.transform.SetParent(chatBox.transform, false);
        // newChat.transform.position = new Vector3(newChat.transform.position.x, newChat.transform.position.y, 0);

        newChat.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = _message;
        // Vector2 vec = new Vector2(newChat.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().preferredWidth + 30f, newChat.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().preferredHeight + 5f);
        // newChat.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = vec;
        chatInput.gameObject.SetActive(false);
        chatInput.text = "";

        if (nicknameText != null) nicknameText.transform.SetAsLastSibling();
    }

    [PunRPC]
    public void ShowChattingSign()
    {
        chattingSign = Instantiate(chatPrefab, chatBox.transform.position, Quaternion.Euler(0, 0, 0));
        chattingSign.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "...";
        chattingSign.transform.SetParent(chatBox.transform, false);
        chattingSign.GetComponentInChildren<Chat>().isChat = false;
    }

    [PunRPC]
    public void CloseChattingSign()
    {
        chattingSign.GetComponentInChildren<Chat>().chatAnimator.SetTrigger("TurnOff");
    }

    void LateUpdate()
    {

    }

    void Update()
    {
        if (nicknameText == null)
        {
            nicknameText = Instantiate(nicknamePrefab, chatBox.transform.position, Quaternion.identity);
            nicknameText.transform.SetParent(chatBox.transform, false);
            nicknameText.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = photonView.Owner.NickName;
            nicknameText.SetActive(false);
        }

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
                photonView.RPC("CloseChattingSign", RpcTarget.Others);
            }
            else
            {
                chatInput.gameObject.SetActive(true);
                photonView.RPC("ShowChattingSign", RpcTarget.Others);
            }
            chatInput.ActivateInputField();
            chatInput.Select();
        }

        if (Input.GetKeyDown(KeyCode.N) && photonView.IsMine)
        {
            ActivateNickname(3.0f);
        }
    }
}
