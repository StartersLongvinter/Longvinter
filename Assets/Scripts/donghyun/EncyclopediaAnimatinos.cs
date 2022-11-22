using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaAnimatinos : MonoBehaviour
{
    private Sequence initDot;
    private Sequence enableDot;
    private Sequence disableDot;

    private void Awake()
    {
        // initDot = DOTween.Sequence();
        // enableDot = DOTween.Sequence();
        // disableDot = DOTween.Sequence();
        //
        // transform.localScale = new Vector3(0, 0, 0);
        //
        // initDot
        //     .Prepend(transform.DOLocalMove(Vector3.left * -100f, 0.5f));
        //
        // enableDot
        //     .Prepend(transform.DOScale(new Vector3(0f, 0f, 0f), 0.01f))
        //     .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f))
        //     .Join(transform.DOLocalMove(Vector3.left * 500f, 0.5f));
        //
        // disableDot
        //     .Prepend(transform.DOLocalMove(Vector3.left * -100f, 0.5f))
        //     .Join(transform.DOScale(new Vector3(0f, 0f, 0f), 0.1f));
        //
        // initDot.Play();
    }

    private void Start()
    {
        initDot = DOTween.Sequence();

        transform.localScale = new Vector3(0, 0, 0);
        
        initDot
            .SetAutoKill(false)
            .SetLink(gameObject, LinkBehaviour.RestartOnEnable)
            .Append(transform.DOLocalMove(Vector3.left * -100f, 0.5f))
            .Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f))
            .Join(transform.DOLocalMove(Vector3.left * 500f, 0.5f));
    }

    // private void OnEnable()
    // {
    //     enableDot.Play();
    // }
    //
    // private void OnDisable()
    // {
    //     disableDot.Play();
    // }
    //
    // private void OnDestroy()
    // {
    //     initDot.Kill();
    //     enableDot.Kill();
    //     disableDot.Kill();
    // }
}