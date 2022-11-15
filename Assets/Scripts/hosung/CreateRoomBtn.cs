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

    void Awake()
    {
        
    }

    public void CreateRoom()
    {
        string password = inputPassword.text;
        int maxPlayercount = Mathf.RoundToInt(maxPlayerSlider.value);
        bool isPVP = pvpToggle.isOn;
        NetworkManager.instance.OnClickCreate(maxPlayercount, isPVP, password);
    }

    void Update()
    {
        inputPassword.interactable = lockToggle.isOn;
        maxPlayerValue.text = Mathf.RoundToInt(maxPlayerSlider.value).ToString();

        if (PhotonNetwork.InRoom && (int)PhotonNetwork.CurrentRoom.CustomProperties["maxPlayers"] != 1)
        {
            multiplayButton.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
