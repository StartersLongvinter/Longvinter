using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            //eventData.pointerDrag.transform.SetParent(transform);
            // Debug.Log(eventData.pointerDrag.GetComponent<RectTransform>().position);

            //eventData.pointerDrag.GetComponent<RectTransform>().position = Vector3.zero;
            
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position =
                GetComponent<RectTransform>().position;
        }
    }
}
