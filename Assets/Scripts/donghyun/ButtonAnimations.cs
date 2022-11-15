using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimations : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_InputField inputField;

    private Color currentColor;
    private Sequence buttonSeq;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        currentColor = button.GetComponent<Image>().color;
        Color wrongColor = new Color(255, 89, 79);
        
        button.onClick.AddListener((() =>
        {
            if (inputField.text.Equals(String.Empty))
            {
                // 닷트원 시퀀스 사용하지 않은것
                // thisBtn.transform.DOPunchPosition(
                //     new Vector3(50,  0, 0),
                //     0.5f);
                //
                // thisBtn.image.DOColor(Color.red, 1f);
                // thisBtn.image.DOColor(currentColor, 1f);

                buttonSeq = DOTween.Sequence()
                    .SetAutoKill(false)
                    .Prepend(button.transform.DOPunchPosition(new Vector3(50, 0, 0), 0.5f))
                    .Join(button.GetComponent<Image>().DOColor(Color.red, 0.1f))
                    .Append(button.GetComponent<Image>().DOColor(currentColor, 0.1f));
            }
            else
            {
                buttonSeq = DOTween.Sequence()
                    .SetAutoKill(false)
                    .Prepend(button.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f))
                    .Insert(0.1f, button.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f))
                    .AppendCallback((() =>
                    {
                        switch (gameObject.name)
                        {
                            case "StartBtn":
                                NetworkManager.instance.OnClickStart();
                                break;
                            
                            case "ConnectServerBtn":
                                NetworkManager.instance.OnClickServer();
                                break;
                            
                            case "ConnectBtn":
                                NetworkManager.instance.OnClickCreate();
                                break;
                        }
                    }));
            }
        }));
    }
}
