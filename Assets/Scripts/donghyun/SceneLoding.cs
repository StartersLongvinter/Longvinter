using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoding : MonoBehaviour
{
    [SerializeField] GameObject loadingPanel;

    private float timer;
    private GameObject DotPos;

    private Sequence tween;

    private void Awake()
    {
        loadingPanel = GameObject.Find("Canvas").transform.GetChild(10).gameObject;
        DotPos = loadingPanel.transform.GetChild(5).gameObject;
    }
    
    private void Start()
    {
        tween = DOTween.Sequence();

        tween.SetAutoKill(false);
        
        DotPos.SetActive(true);
        
        loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "캐릭터를 로딩중입니다";

        Vector2 vec = new Vector2(loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().preferredWidth,
            loadingPanel.transform.GetChild(4).GetChild(0).GetComponent<Text>().preferredHeight);
        
        loadingPanel.transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = vec;

        DotPos.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        DotPos.GetComponent<RectTransform>().position = new Vector3( loadingPanel.transform.GetChild(4).position.x + 10 + vec.x / 2, DotPos.transform.position.y);

        
        StartCoroutine(WaitTime());
    }

    private void Update()
    {
        if (timer >= 1f)
        {
            if (!tween.active)
            {
                loadingPanel.SetActive(false);
            }
        }
    }

    IEnumerator WaitTime()
    {
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        tween
            .Prepend(loadingPanel.GetComponent<CanvasGroup>().DOFade(0, 1)).AppendCallback((() =>
            {
                tween.Kill();
            }));
    }
}
