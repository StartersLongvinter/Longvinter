using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InventoryAnimations : MonoBehaviour
{
    private Sequence Dot;
    private void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
        Dot = DOTween.Sequence();
        
        Dot.Prepend(transform.DOLocalMove(Vector3.right * -100f, 0.5f));
    }

    private void OnEnable()
    {
            Dot
            .Prepend(transform.DOScale(new Vector3(0f, 0f, 0f), 0.01f))
            .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f))
            .Join(transform.DOLocalMove(Vector3.right * 500f, 0.5f));
    }

    private void OnDisable()
    {
            Dot
            .Prepend(transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
            .Join(transform.DOScale(new Vector3(0f, 0f, 0f), 0.1f));
    }
}
