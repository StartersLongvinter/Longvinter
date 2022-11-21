using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private Transform previousPos;
    private Canvas canvas;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GameObject.Find("Inventory Canvas").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousPos = transform.parent;
        
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
        
        transform.SetParent(transform.parent.parent);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
        //rect.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent.tag != "ItemSlot")
        {
            transform.SetParent(previousPos);
            rect.position = previousPos.GetComponent<RectTransform>().position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }
}
