using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviour
{
    public ItemData itemData;

    [SerializeField] private GameObject[] fish;
    [SerializeField] private GameObject[] feathers;
    [SerializeField] private GameObject[] plants;
    [SerializeField] private GameObject encyclopediaNotiPrefab;
    [SerializeField] private GameObject newItemNotiPrefab;
    
    [SerializeField] private Transform notificationPos;
   

    private bool isCheckedEncyclopedia;
    
    public void GainItem()
    {
        if (itemData.itClassify == ItemData.ItemClassify.Fish)
        {
            for (int i = 0; i < fish.Length; i++)
            {
                if (itemData.itName.Equals(fish[i].gameObject.name))
                {
                    Debug.Log(fish[i].GetComponent<Image>().color.a);
                    if (fish[i].GetComponent<Image>().color.a != 1) //도감의 알파값이 1이 아니라면 새로운 아이템을 찾은것
                    {
                        Color newColor = fish[i].GetComponent<Image>().color;
                        fish[i].GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, 1);
                        
                        //새로운 아이템을 찾았으므로 encyclopediaNotiPrefab, newItemNotiPrefab 둘다 켜져야함
                        NotificationManager.instance.FillEncyclopedia();
                        NotificationManager.instance.NewItemGainNotification(itemData);
                    }
                    //도감의 알파값이 1이라면 도감이 채워진것이므로 newItemNotiPrefab만 켜주면됨
                    else
                    {
                        NotificationManager.instance.NewItemGainNotification(itemData);
                    }
                }
            }
        }
        else if (itemData.itClassify == ItemData.ItemClassify.Feather)
        {
            for (int i = 0; i < feathers.Length; i++)
            {
                if (itemData.itName.Equals(feathers[i].gameObject.name))
                {
                    if (feathers[i].GetComponent<Image>().color.a != 1) //도감의 알파값이 1이 아니라면 새로운 아이템을 찾은것
                    {
                        Color newColor = feathers[i].GetComponent<Image>().color;
                        feathers[i].GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, 1);
                            
                        //새로운 아이템을 찾았으므로 encyclopediaNotiPrefab, newItemNotiPrefab 둘다 켜져야함
                        NotificationManager.instance.FillEncyclopedia();
                        NotificationManager.instance.NewItemGainNotification(itemData);
                    }
                    //도감의 알파값이 1이라면 도감이 채워진것이므로 newItemNotiPrefab만 켜주면됨
                    else
                    {
                        NotificationManager.instance.NewItemGainNotification(itemData);
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < plants.Length; i++)
            {
                if (itemData.itName.Equals(plants[i].gameObject.name))
                {
                    if (plants[i].GetComponent<Image>().color.a != 1) //도감의 알파값이 1이 아니라면 새로운 아이템을 찾은것
                    {
                        Color newColor = plants[i].GetComponent<Image>().color;
                        plants[i].GetComponent<Image>().color = new Color(newColor.r, newColor.g, newColor.b, 1);
                            
                        //새로운 아이템을 찾았으므로 encyclopediaNotiPrefab, newItemNotiPrefab 둘다 켜져야함
                        NotificationManager.instance.FillEncyclopedia();
                        NotificationManager.instance.NewItemGainNotification(itemData);
                    }
                    //도감의 알파값이 1이라면 도감이 채워진것이므로 newItemNotiPrefab만 켜주면됨
                    else
                    {
                        NotificationManager.instance.NewItemGainNotification(itemData);
                    }
                }
            }
        }
    }
}
