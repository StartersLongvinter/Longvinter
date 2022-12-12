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
    [SerializeField] Font noticeFont;

    float noticeTime = 0;

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

        foreach (string _badText in NetworkManager.Instance.badText)
        {
            if (_message.Contains(_badText))
            {
                _message = _message.Replace(_badText, "nice");
                Debug.Log($"chat '{_badText}' is changed 'nice'");
            }
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
    public void SendAnAnnouncement(string _message, float _timeValue)
    {
        GameObject noticeObject = GameObject.Find("Notice");
        if (noticeObject == null)
        {
            noticeObject = new GameObject("Notice");
            noticeObject.AddComponent<Text>();
            noticeObject.transform.SetParent(GameObject.Find("Canvas").transform);
        }
        Text noticeText = noticeObject.GetComponent<Text>();
        noticeText.rectTransform.sizeDelta = new Vector2(1200, 100);
        noticeText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        noticeText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        noticeText.rectTransform.pivot = new Vector2(0.5f, 1f);
        noticeText.rectTransform.anchoredPosition = new Vector3(0, -100f, 0);
        noticeText.font = noticeFont;
        noticeText.fontStyle = FontStyle.Bold;
        noticeText.alignment = TextAnchor.UpperCenter;

        noticeText.text = string.IsNullOrEmpty(_message) ? "" : "<size=60><color=red>" + _message + "</color></size>";
        if (PhotonNetwork.IsMasterClient) noticeTime = _timeValue;

        chatInput.gameObject.SetActive(false);
        chatInput.text = "";
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
        if (noticeTime != 0)
        {
            noticeTime -= Time.deltaTime;
            if (noticeTime <= 0)
            {
                photonView.RPC("SendAnAnnouncement", RpcTarget.AllViaServer, "", 0f);
                noticeTime = 0;
            }
        }

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
                if (chatInput.text.Length >= 3 && chatInput.text.Substring(0, 3) == "/nt" && PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SendAnAnnouncement", RpcTarget.AllViaServer, chatInput.text.Substring(3), 3f);
                }
                else
                {
                    photonView.RPC("SendNewMessage", RpcTarget.All, chatInput.text);
                }
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
    }
}
