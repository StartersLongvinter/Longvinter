using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TextVerticalSize : MonoBehaviour
{
    [SerializeField] bool isTMPro = true;
    [SerializeField] bool isInputField = false;
    void Awake()
    {

    }

    void Update()
    {
        if (isTMPro)
        {
            if (isInputField)
            {
                if (string.IsNullOrEmpty(GetComponent<TMP_InputField>().text))
                {
                    transform.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                    return;
                }
                Vector2 vec = new Vector2(transform.GetComponent<TMP_InputField>().preferredWidth + 30f, 80f);
                transform.GetComponent<RectTransform>().sizeDelta = vec;
            }
            else
            {
                Vector2 vec = new Vector2(transform.GetComponentInChildren<TextMeshProUGUI>().preferredWidth + 30f, transform.GetComponentInChildren<TextMeshProUGUI>().preferredHeight);
                transform.GetComponent<RectTransform>().sizeDelta = vec;
            }
        }
        else
        {
            if (isInputField)
            {
                if (string.IsNullOrEmpty(GetComponent<InputField>().text))
                {
                    transform.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                    return;
                }
                Vector2 vec = new Vector2(transform.GetComponent<InputField>().preferredWidth + 30f, 80f);
                transform.GetComponent<RectTransform>().sizeDelta = vec;
            }
            else
            {
                Vector2 vec = new Vector2(transform.GetComponentInChildren<Text>().preferredWidth + 30f, transform.GetComponentInChildren<Text>().preferredHeight);
                transform.GetComponent<RectTransform>().sizeDelta = vec;
            }
        }
    }
}
