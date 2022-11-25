using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toolTip;
    
    private GameObject slot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(true);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid)
        {
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>() != null)
            {
                toolTip.SetActive(true);
                
                slot = eventData.pointerCurrentRaycast.gameObject;

                slot.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0f);

                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().item != null)
                {
                    toolTip.GetComponentInChildren<Text>().text = eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().item.itKorName;
                }
                
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().equipment != null)
                {
                    toolTip.GetComponentInChildren<Text>().text = eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().equipment.emKorName;
                }
                
                Vector2 vec = new Vector2(toolTip.GetComponentInChildren<Text>().preferredWidth + 30f, toolTip.GetComponentInChildren<Text>().preferredHeight);

                toolTip.GetComponent<RectTransform>().sizeDelta = vec;
                
                toolTip.GetComponent<RectTransform>().position =
                    new Vector2(eventData.position.x + 30f, eventData.position.y + 30f);
            }
            else
            {
                slot.transform.DOScale(new Vector3(1f, 1f, 1f), 0f);
                toolTip.SetActive(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slot = null;
        toolTip.gameObject.SetActive(false);
    }
}
