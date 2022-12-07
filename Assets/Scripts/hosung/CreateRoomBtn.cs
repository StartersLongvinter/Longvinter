using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateRoomBtn : MonoBehaviour
{
    [SerializeField] Toggle pvpToggle;
    [SerializeField] Slider maxPlayerSlider;
    [SerializeField] TextMeshProUGUI maxPlayerValue;
    [SerializeField] Toggle lockToggle;
    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] Button multiplayButton;
    [SerializeField] Button playerListButton;
    [SerializeField] GameObject multiplayPanel;
    [SerializeField] Button createRoomButton;

    private bool isChanged = false;

    void Awake()
    {
        maxPlayerSlider.onValueChanged.AddListener(delegate { isChanged = true; });
    }

    private void OnEnable()
    {
        isChanged = false;
    }

    public void CreateRoom()
    {
        string password = inputPassword.text;
        int maxPlayercount = Mathf.RoundToInt(maxPlayerSlider.value);
        bool isPVP = pvpToggle.isOn;
        NetworkManager.Instance.OnClickCreate(PhotonNetwork.LocalPlayer.NickName, maxPlayercount, isPVP, password);

        multiplayPanel.SetActive(false);
        multiplayButton.gameObject.SetActive(false);
    }

    void Update()
    {
        createRoomButton.interactable = isChanged;

        inputPassword.interactable = lockToggle.isOn;
        maxPlayerValue.text = Mathf.RoundToInt(maxPlayerSlider.value).ToString();

        if (PhotonNetwork.InRoom && (int)PhotonNetwork.CurrentRoom.CustomProperties["maxPlayers"] > 1)
        {
            multiplayButton.gameObject.SetActive(false);
            playerListButton.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
