using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaAnimatinos : MonoBehaviour
{
    private void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
        DOTween.Sequence()
            .Prepend(transform.DOLocalMove(Vector3.left * -100f, 0.5f));
    }

    private void OnEnable()
    {
        DOTween.Sequence()
            .Prepend(transform.DOScale(new Vector3(0f, 0f, 0f), 0.01f))
            .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f))
            .Join(transform.DOLocalMove(Vector3.left * 500f, 0.5f));
    }

    private void OnDisable()
    {
        DOTween.Sequence()
            .Prepend(transform.DOLocalMove(Vector3.left * -100f, 0.5f))
            .Join(transform.DOScale(new Vector3(0f, 0f, 0f), 0.1f));
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
