using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateRoomBtn : MonoBehaviour
{
    [SerializeField] private GameObject adminPanel;
    [SerializeField] Slider maxPlayerSlider;
    [SerializeField] Text maxPlayerValue;
    [SerializeField] InputField inputPassword;
    [SerializeField] InputField inputServerName;
    [SerializeField] Button makeRoomBtn;
    
    void Awake()
    {
        makeRoomBtn.onClick.AddListener((() =>
        {
            CreateRoom();
            adminPanel.SetActive(false);
        }));
    }

    private void OnEnable()
    {
    }

    public void CreateRoom()
    {
        string password = inputPassword.text;
        int maxPlayercount = Mathf.RoundToInt(maxPlayerSlider.value);
        NetworkManager.Instance.OnClickCreate(inputServerName.text, maxPlayercount, password);
    }

    void Update()
    {
        maxPlayerValue.text = Mathf.RoundToInt(maxPlayerSlider.value).ToString();
    }
}
