using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBagInventory : MonoBehaviour
{
    [SerializeField] private GameObject[] rightDeadBagInventory;
    [SerializeField] private GameObject[] leftDeadBagInventory;

    List<GameObject> currentItemList = new List<GameObject>();
    List<GameObject> deadItemList = new List<GameObject>();

    PlayerStat playerstat;
    //PlayerInventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        //playerstat = GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void OnTriggerStay(Collider other)
    {
        //inventory = other.gameObject.GetComponent<PlayerInventory>();
    }

    //생성되면 원래 있던 playerinventory의 itemlist를 가져옴 그거대로 for문 돌며 자식에 instantiate 시켜주자
    private void OnEnable()
    {
        currentItemList = transform.root.gameObject.GetComponent<PlayerInventory>().itemList;
        deadItemList = transform.root.gameObject.GetComponent<PlayerController>().selectedDeadBag.GetComponent<DeadItemList>().deadItems;

        for (int i = 0; i < currentItemList.Count; i++)
        {
            GameObject go = currentItemList[i];

            if (go.GetComponent<Item>().item != null)
            {
                GameObject temp = Instantiate(go.GetComponent<Item>().item.itPrefab, rightDeadBagInventory[i].transform);
                temp.transform.localScale = new Vector3(1, 1, 1);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                continue;
            }

            else
            {
                GameObject temp =
                Instantiate(go.GetComponent<Item>().equipment.eqCanvasPrefab, rightDeadBagInventory[i].transform);
                temp.transform.localScale = new Vector3(1, 1, 1);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                continue;
            }
        }

        for (int i = 0; i < deadItemList.Count; i++)
        {
            GameObject go = deadItemList[i];

            if (go.GetComponent<Item>().item != null)
            {
                GameObject temp = Instantiate(go.GetComponent<Item>().item.itPrefab, leftDeadBagInventory[i].transform);
                temp.transform.localScale = new Vector3(1, 1, 1);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                continue;
            }

            else
            {
                GameObject temp =
                Instantiate(go.GetComponent<Item>().equipment.eqCanvasPrefab, leftDeadBagInventory[i].transform);
                temp.transform.localScale = new Vector3(1, 1, 1);
                temp.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                continue;
            }
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
        for (int i = 0; i < rightDeadBagInventory.Length; i++)
        {
            if (rightDeadBagInventory[i].transform.childCount == 0) break;

            Destroy(rightDeadBagInventory[i].transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < leftDeadBagInventory.Length; i++)
        {
            if (leftDeadBagInventory[i].transform.childCount == 0) break;

            Destroy(leftDeadBagInventory[i].transform.GetChild(0).gameObject);
        }
    }
}
