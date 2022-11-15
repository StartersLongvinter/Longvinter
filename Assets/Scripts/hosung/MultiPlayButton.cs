using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MultiPlayButton : MonoBehaviour
{
    [SerializeField] GameObject multiplayPanel;

    void Awake()
    {
        
    }

    public void OnClickMultiplayButton()
    {
        multiplayPanel.SetActive(!multiplayPanel.activeSelf);
    }

    void Update()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) this.gameObject.SetActive(false);
    }
}
