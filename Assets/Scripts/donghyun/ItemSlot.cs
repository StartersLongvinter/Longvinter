using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private GameObject toolTip;

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
            transform.GetChild(0).DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0f);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid && eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>())
        {
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().item != null)
            {
                toolTip.GetComponentInChildren<Text>().text = eventData.pointerCurrentRaycast.gameObject
                    .GetComponent<Item>().item.itKorName;

                Vector2 vec = new Vector2(toolTip.GetComponentInChildren<Text>().preferredWidth + 30f,
                    toolTip.GetComponentInChildren<Text>().preferredHeight);

                toolTip.GetComponent<RectTransform>().sizeDelta = vec;

                toolTip.GetComponent<RectTransform>().position =
                    new Vector2(eventData.position.x + 30f, eventData.position.y + 30f);
            }

            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Item>().equipment != null)
            {
                toolTip.GetComponentInChildren<Text>().text = eventData.pointerCurrentRaycast.gameObject
                    .GetComponent<Item>().equipment.emKorName;

                Vector2 vec = new Vector2(toolTip.GetComponentInChildren<Text>().preferredWidth + 30f,
                    toolTip.GetComponentInChildren<Text>().preferredHeight);

                toolTip.GetComponent<RectTransform>().sizeDelta = vec;

                toolTip.GetComponent<RectTransform>().position =
                    new Vector2(eventData.position.x + 30f, eventData.position.y + 30f);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        
        if (transform.childCount != 0)
        {
            transform.GetChild(0).DOScale(new Vector3(1f, 1f, 1f), 0f);
        }
    }
}