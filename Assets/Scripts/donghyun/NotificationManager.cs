using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    
    [SerializeField] private Transform notificationPos;
    [SerializeField] private Transform sustainableItemUseNotificationPos;
    
    [SerializeField] private GameObject encyclopediaNotiPrefab;
    [SerializeField] private GameObject newItemNotiPrefab;
    [SerializeField] GameObject fishingNotiPrefab;

    private void Awake()
    {
        if (instance != null) return;

        instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    public void FillEncyclopedia()
    {
        Instantiate(encyclopediaNotiPrefab, notificationPos);
    }

    public void WarningNotification()
    {
        GameObject noti = Instantiate(fishingNotiPrefab);
        noti.transform.SetParent(notificationPos);
    }

    public void NewItemGainNotification(ItemData item)
    {
        GameObject noti = Instantiate(newItemNotiPrefab);
        noti.transform.GetChild(0).GetComponent<Image>().sprite = item.itImage;
        noti.transform.GetChild(1).GetComponent<Text>().text = item.itKorName;
        noti.transform.GetChild(2).GetComponent<Text>().text = item.itExplan;
        noti.transform.SetParent(notificationPos);
    }

    public GameObject ItemUseNotification(GameObject item)
    {
        GameObject effect = Instantiate(item.GetComponent<Item>().item.itEffectPrefab);
        effect.transform.SetParent(sustainableItemUseNotificationPos);

        return effect;
    }
}
