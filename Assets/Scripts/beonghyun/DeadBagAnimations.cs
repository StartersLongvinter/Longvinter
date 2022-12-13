using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeadBagAnimations : MonoBehaviour
{
    private Sequence initDot;
    private Sequence enableDot;
    private Sequence disableDot;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

    private void Start()
    {
        initDot = DOTween.Sequence();

        //transform.localScale = new Vector3(0, 0, 0);

        initDot
            .SetAutoKill(false)
            .SetLink(gameObject, LinkBehaviour.RestartOnEnable)
            .Append(transform.DOLocalMove(Vector3.right * -50f, 0.5f))
            .Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f))
            .Join(transform.DOLocalMove(Vector3.right * 50f, 0.5f));
    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }
}
