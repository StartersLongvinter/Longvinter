using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NotificationAnimations : MonoBehaviour
{
    private Sequence Dot;
    
    private void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
        Dot = DOTween.Sequence();
    }

    private void OnEnable()
    {
            Dot
            .Prepend(transform.DOScale(new Vector3(0f, 0f, 0f), 0.01f))
            .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
    }

    private void Update()
    {
        Invoke("DestroyGameObject", 4f);
    }

    void DestroyGameObject()
    {
        Dot.Kill();
        Destroy(gameObject);
    }
}
