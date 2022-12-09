using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.UI;

public class Title : MonoBehaviourPun
{
    public string playerPrefabName = "";
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject networkManagerObject;
    [SerializeField] private GameObject loadingPanel;

    private GameObject DotPos;
    private float timer;

    private bool isConnected;

    void Awake()
    {
        if (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
            networkManagerObject.SetActive(true);
        NetworkManager.Instance.Init(playerPrefabName, roomPrefab);

        DotPos = loadingPanel.transform.GetChild(5).gameObject;
    }

    private void Start()
    {
        DotPos.SetActive(true);
    }

    void Update()
    {
        loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = NetworkManager.Instance.currentConnectionStatus;

        Vector2 vec = new Vector2(loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().preferredWidth,
            loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().preferredHeight);
        
        loadingPanel.transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = vec;

        DotPos.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        DotPos.GetComponent<RectTransform>().position = new Vector3( loadingPanel.transform.GetChild(4).position.x + 10 + vec.x / 2, DotPos.transform.position.y);
        
        

        if (loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().text.Equals("서버에 연결되었습니다") && !isConnected)
        {
            isConnected = true;
            StartCoroutine(WaitForTimer());
        }
    }

    IEnumerator WaitForTimer()
    {
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Sequence tween = DOTween.Sequence();
        tween.Prepend(loadingPanel.GetComponent<CanvasGroup>().DOFade(0, 1)).AppendCallback((() =>
        {
            loadingPanel.SetActive(false);
        }));
    }
}
