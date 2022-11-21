using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int MAXITEM = 16;
    
    [SerializeField] private GameObject[] bagInventory;
    [SerializeField] private GameObject[] equipmentInventory;
    [SerializeField] private GameObject[] encyclopedia;
    
    public List<ItemData> itemList = new List<ItemData>();

    private int currentItemList;

    private bool isItemUpdated;
    // Start is called before the first frame update
    void Start()
    {
        currentItemList = itemList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentItemList < itemList.Count && itemList.Count < MAXITEM && !isItemUpdated)
        {
            isItemUpdated = true;
            currentItemList++;

            GameObject item;

            if (itemList[itemList.Count - 1].itType == ItemData.ItemType.Food)
            {
                for (int i = 0; i < bagInventory.Length; i++)
                {
                    if (bagInventory[i].transform.childCount == 0)
                    {
                        item = Instantiate(itemList[itemList.Count - 1].itPrefab, bagInventory[i].transform);
                        item.GetComponent<RectTransform>().position =
                            bagInventory[i].GetComponent<RectTransform>().position;

                        item.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                        break;
                    }
                }
            }

            isItemUpdated = false;
        }
    }
}
