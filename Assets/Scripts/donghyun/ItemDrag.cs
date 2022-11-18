using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private Transform previousPos;
    private Canvas canvas;

    private bool isClicked;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GameObject.Find("Inventory Canvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isClicked = true;
        }

        // if (isClicked)
        // {
        //     isClicked = false;
        //     
        //     RaycastHit[] hits;
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //         
        //     hits = Physics.RaycastAll(ray);
        //
        //     var distinctHits = hits.DistinctBy(x => x.collider.name);
        //         
        //     foreach (var hit in distinctHits)
        //     {
        //         Debug.Log(hit.collider.name);
        //         if (hit.collider.GetComponent<Item>() != null)
        //         {
        //             Debug.Log(hit.collider.GetComponent<Item>().item.itName);
        //             Destroy(gameObject);
        //         }
        //     }
        // }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log(gameObject.GetComponent<Item>().item.itKorName + "사용함");
            Destroy(gameObject);
        }
    }
}
