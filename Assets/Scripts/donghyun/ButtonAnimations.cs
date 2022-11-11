using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimations : MonoBehaviour
{
    [SerializeField] private Button ConnectBtn;

    private Color currentColor;
    private Sequence buttonSeq;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        currentColor = ConnectBtn.colors.normalColor;
        
        ConnectBtn.onClick.AddListener((() =>
        {
            // ConnectBtn.transform.DOPunchPosition(
            //     new Vector3(50,  0, 0),
            //     0.5f);
            
            buttonSeq = DOTween.Sequence();

            buttonSeq.Prepend(ConnectBtn.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f));
            buttonSeq.Join(ConnectBtn.GetComponent<Image>().DOColor(Color.red, 0.1f));
            
            //buttonSeq.AppendInterval(0.1f);
            
            buttonSeq.Insert(0.1f ,ConnectBtn.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
            buttonSeq.Append(ConnectBtn.GetComponent<Image>().DOColor(currentColor, 1f));

            buttonSeq.Play();
        }));
    }
}
