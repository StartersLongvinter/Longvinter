using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toolTip;

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
                toolTip.GetComponent<RectTransform>().position =
                    new Vector2(eventData.position.x + 30f, eventData.position.y + 30f);
                toolTip.GetComponentInChildren<Text>().text =
                    eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().item.itKorName;
            }
            else
            {
                toolTip.SetActive(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
    }
}
