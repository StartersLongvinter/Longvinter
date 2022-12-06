using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toolTip;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemExplainText;
    [SerializeField] private GameObject explainBG;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position =
                GetComponent<RectTransform>().position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid &&
            eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>() != null)
        {
            toolTip.SetActive(true);
            explainBG.SetActive(true);
            
            transform.GetChild(0).DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0f);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid && eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>())
        {
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().item != null)
            {
                itemNameText.text = eventData.pointerCurrentRaycast.gameObject
                    .GetComponent<Item>().item.itKorName;

                Vector2 vec = new Vector2(itemNameText.preferredWidth + 30f,
                    itemNameText.preferredHeight);

                toolTip.GetComponent<RectTransform>().sizeDelta = vec;

                if (vec.x + eventData.position.x >= Screen.width)
                {
                    toolTip.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x, 
                            eventData.position.y + (vec.y));
                }
                else
                {
                    toolTip.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x + (vec.x / 2) + 20f, eventData.position.y + 50f);
                }
            }

            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().equipment != null)
            {
                itemNameText.text = eventData.pointerCurrentRaycast.gameObject
                    .GetComponent<Item>().equipment.emKorName;

                Vector2 vec = new Vector2(itemNameText.preferredWidth + 30f,
                    itemNameText.preferredHeight);

                toolTip.GetComponent<RectTransform>().sizeDelta = vec;

                if (vec.x + eventData.position.x >= Screen.width)
                {
                    toolTip.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x, 
                            eventData.position.y + (vec.y));
                }
                else
                {
                    toolTip.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x + (vec.x / 2) + 20f, eventData.position.y + 50f);
                }
            }
        }
        
        if (eventData.pointerCurrentRaycast.isValid 
            && eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>())
        {
            itemExplainText.gameObject.SetActive(true);

            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().item != null)
            {
                itemExplainText.text = eventData.pointerCurrentRaycast.gameObject
                    .GetComponent<Item>().item.itExplan;

                Vector2 vec = new Vector2(itemExplainText.preferredWidth + 30f,
                    itemExplainText.preferredHeight);
                
                explainBG.GetComponent<RectTransform>().sizeDelta = vec;

                if (vec.x + eventData.position.x >= Screen.width)
                {
                    explainBG.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x, 
                            eventData.position.y - (vec.y / 2));
                }
                else
                {
                    explainBG.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x + (vec.x / 2) + 20f, eventData.position.y - (vec.y / 2));
                }
            }

            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().equipment != null)
            {
                itemExplainText.text = eventData.pointerCurrentRaycast.gameObject
                    .GetComponent<Item>().equipment.emKorName;

                Vector2 vec = new Vector2(itemExplainText.preferredWidth + 30f,
                    itemExplainText.preferredHeight);

                explainBG.GetComponent<RectTransform>().sizeDelta = vec;

                if (vec.x + eventData.position.x >= Screen.width)
                {
                    explainBG.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x, 
                            eventData.position.y - (vec.y / 2));
                }
                else
                {
                    explainBG.GetComponent<RectTransform>().position =
                        new Vector2(eventData.position.x + (vec.x / 2) + 20f, eventData.position.y - (vec.y / 2));
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
        explainBG.SetActive(false);
        
        itemExplainText.text = String.Empty;

        if (transform.childCount != 0)
        {
            transform.GetChild(0).DOScale(new Vector3(1f, 1f, 1f), 0f);
        }
    }
}