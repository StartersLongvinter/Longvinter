using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun
{
    public int MAXITEM = 16;
    
    [SerializeField] private GameObject[] bagInventory;
    [SerializeField] private GameObject[] equipmentInventory;
    [SerializeField] private GameObject[] encyclopedia;

    public List<GameObject> itemList = new List<GameObject>();
    public List<GameObject> equipmentList = new List<GameObject>();
    
    public int inventoryCount = 0;

    private bool isItemUpdated;
    // Start is called before the first frame update
    void Start()
    {
        // currentEquipListCount = equipmentList.Count;
        //
        // inventoryCount = currentItemListCount + currentEquipListCount;
    }

    // Update is called once per frame
    void Update()
    {
        

        // if (currentEquipListCount < equipmentList.Count && equipmentList.Count <= MAXITEM && !isItemUpdated)
        // {
        //     isItemUpdated = true;
        //     currentEquipListCount++;
        //     inventoryCount += currentEquipListCount;
        //
        //     GameObject item;
        //     
        //     for (int i = 0; i < bagInventory.Length; i++)
        //     {
        //         if (bagInventory[i].transform.childCount == 0)
        //         {
        //             item = Instantiate(equipmentList[equipmentList.Count - 1].itPrefab, bagInventory[i].transform);
        //             item.transform.localScale = new Vector3(1, 1, 1);
        //             item.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
        //             item.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
        //
        //             break;
        //         }
        //     }
        //     isItemUpdated = false;
        // }
    }

    public void AddItem(GameObject go)
    {
        if (itemList.Count <= MAXITEM)
        {
            if (go.GetComponent<Item>().item != null)
            {
                GameObject item;
                
                for (int i = 0; i < bagInventory.Length; i++)
                {
                    if (bagInventory[i].transform.childCount == 0)
                    {
                        item = Instantiate(go.GetComponent<Item>().item.itPrefab, bagInventory[i].transform);
                        item.transform.localScale = new Vector3(1, 1, 1);
                        item.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                        item.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                        itemList.Add(item);
                        break;
                    }
                }
            }
            else if (go.GetComponent<Item>().equipment != null)
            {
                GameObject item;
            
                for (int i = 0; i < bagInventory.Length; i++)
                {
                    if (bagInventory[i].transform.childCount == 0)
                    {
                        item = Instantiate(go.GetComponent<Item>().equipment.itPrefab, bagInventory[i].transform);
                        item.transform.localScale = new Vector3(1, 1, 1);
                        item.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                        item.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                        itemList.Add(item);
                        break;
                    }
                }
            }
            inventoryCount = itemList.Count;
        }
        
        go.GetComponent<Item>().CallDestroyGameObject();
    }

    //가방에서 장비를 눌렀을 경우 장비창으로 장비가 넘어가게 되는데 이때 가방속 아이템들을 위치를 당겨주는 함수
    public void updateBagInventory()
    {
        for (int itemIndex = 0; itemIndex < itemList.Count; itemIndex++)
        {
            for (int i = 0; i < bagInventory.Length; i++)
            {
                if(itemList[itemIndex].transform.parent.Equals(bagInventory[i].transform))
                {
                    break;
                }
                
                if (bagInventory[i].transform.childCount == 0)
                {
                    itemList[itemIndex].transform.SetParent(bagInventory[i].transform);
                    itemList[itemIndex].transform.localScale = new Vector3(1, 1, 1);
                    itemList[itemIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                    itemList[itemIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                    break;
                }
            }
        }
        inventoryCount = itemList.Count;
    }

    //장비창에서 아이템을 눌렀을 경우 가방으로 장비가 넘어가게 되는데 이때 장비 아이템들을 위치를 당겨주는 함수
    public void updateEquipInventory()
    {
        for (int itemIndex = 0; itemIndex < equipmentList.Count; itemIndex++)
        {
            for (int i = 0; i < equipmentInventory.Length; i++)
            {
                if(equipmentList[itemIndex].transform.parent.Equals(equipmentInventory[i].transform))
                {
                    break;
                }
                
                if (equipmentInventory[i].transform.childCount == 0)
                {
                    equipmentList[itemIndex].transform.SetParent(equipmentInventory[i].transform);
                    equipmentList[itemIndex].transform.localScale = new Vector3(1, 1, 1);
                    equipmentList[itemIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                    equipmentList[itemIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                    break;
                }
            }
        }
    }

    public void ItemUse(GameObject go)
    {
        itemList.Remove(go);
        
        updateBagInventory();
        
        Destroy(go);
    }
}
