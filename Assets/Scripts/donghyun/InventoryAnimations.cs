using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InventoryAnimations : MonoBehaviour
{
    private Sequence initDot;
    private Sequence enableDot;
    private Sequence disableDot;
    
    private void Awake()
    {
        
    }

    private void Start()
    {
        initDot = DOTween.Sequence();

        transform.localScale = new Vector3(0, 0, 0);
        
        initDot
            .SetAutoKill(false)
            .SetLink(gameObject, LinkBehaviour.RestartOnEnable)
            .Append(transform.DOLocalMove(Vector3.right * -100f, 0.5f))
            .Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f))
            .Join(transform.DOLocalMove(Vector3.right * 500f, 0.5f));
    }
}
