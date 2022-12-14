using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviourPun
{
    public int MAXITEM = 16;

    [SerializeField] private GameObject[] bagInventory;
    [SerializeField] private GameObject[] equipmentInventory;
    [SerializeField] private GameObject[] encyclopedia;
    
    public List<GameObject> itemList = new List<GameObject>();
    public List<GameObject> equipmentList = new List<GameObject>();
    public List<GameObject> currentUseItem = new List<GameObject>();

    public int inventoryCount = 0;

    public Transform NoOverlapEffectNotificationPos;
    public Transform AfterUseItemNotipos;

    private bool isItemUpdated;
    private bool isStuffed;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WaitForTimer(float time, int idx, GameObject go, GameObject effect)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            effect.GetComponentInChildren<Text>().text = Mathf.Ceil(time).ToString();
            yield return null;
        }

        PlayerStat.LocalPlayer.InitEffect(go.GetComponent<Item>().item.itEffect);

        GameObject afterEffect = Instantiate(go.GetComponent<Item>().item.itAfterExpirePrefab);
        afterEffect.transform.SetParent(AfterUseItemNotipos);

        Destroy(effect);
        Destroy(go);
        currentUseItem.RemoveAt(idx);
    }

    public void AddItem(GameObject go, bool isGroundItem = true)
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
                        item = Instantiate(go.GetComponent<Item>().equipment.eqCanvasPrefab, bagInventory[i].transform);
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

        if (isGroundItem) go.GetComponent<Item>().CallDestroyGameObject();
    }

    //???????????? ????????? ????????? ?????? ??????????????? ????????? ???????????? ????????? ?????? ????????? ??????????????? ????????? ???????????? ??????
    public void updateBagInventory()
    {
        for (int itemIndex = 0; itemIndex < itemList.Count; itemIndex++)
        {
            for (int i = 0; i < bagInventory.Length; i++)
            {
                if (itemList[itemIndex].transform.parent.Equals(bagInventory[i].transform))
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

    //??????????????? ???????????? ????????? ?????? ???????????? ????????? ???????????? ????????? ?????? ?????? ??????????????? ????????? ???????????? ??????
    public void updateEquipInventory()
    {
        for (int itemIndex = 0; itemIndex < equipmentList.Count; itemIndex++)
        {
            for (int i = 0; i < equipmentInventory.Length; i++)
            {
                if (equipmentList[itemIndex].transform.parent.Equals(equipmentInventory[i].transform))
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
        //????????? ??? ?????? ??????????????? ??????
        if (go.GetComponent<Item>().item.itUsable)
        {
            //???????????? ???????????? ????????? ??? ?????? ?????????????????? currentUseItem??? ??????
            if (!go.GetComponent<Item>().item.itCanOverlap)
            {

                GameObject temp = Instantiate(go);
                temp.name = go.GetComponent<Item>().item.itName;
                currentUseItem.Add(temp);
                
                GameObject effect = NotificationManager.instance.ItemUseNotification(go);
                
                StartCoroutine(WaitForTimer(temp.GetComponent<Item>().item.itExpireTime
                    , currentUseItem.IndexOf(temp)
                    , temp
                    , effect));

                switch (go.GetComponent<Item>().item.itEffect)
                {
                    case ItemData.ItemEffect.Health:
                        //??????????????? hp?????? ?????? ????????? ?????????
                        if (go.GetComponent<Item>().item.itIncreaseHealth != 0)
                        {
                            PlayerStat.LocalPlayer.ChangeHp(go.GetComponent<Item>().item.itIncreaseHealth);
                        }

                        break;

                    case ItemData.ItemEffect.ColdDamageReduction:
                        //??????????????? hp?????? ?????? ????????? ?????????
                        if (go.GetComponent<Item>().item.itIncreaseHealth != 0)
                        {
                            PlayerStat.LocalPlayer.ChangeHp(go.GetComponent<Item>().item.itIncreaseHealth);
                        }

                        PlayerStat.LocalPlayer.GetEffect(go.GetComponent<Item>().item.itEffect, go.GetComponent<Item>().item.itApplyPercentage / 100);

                        break;

                    case ItemData.ItemEffect.GetHardAmor:
                        //??????????????? hp?????? ?????? ????????? ?????????
                        if (go.GetComponent<Item>().item.itIncreaseHealth != 0)
                        {
                            PlayerStat.LocalPlayer.ChangeHp(go.GetComponent<Item>().item.itIncreaseHealth);
                        }

                        PlayerStat.LocalPlayer.GetEffect(go.GetComponent<Item>().item.itEffect, go.GetComponent<Item>().item.itApplyPercentage / 100);

                        break;

                    case ItemData.ItemEffect.IncreaseFishingSpeed:
                        //??????????????? hp?????? ?????? ????????? ?????????
                        if (go.GetComponent<Item>().item.itIncreaseHealth != 0)
                        {
                            PlayerStat.LocalPlayer.ChangeHp(go.GetComponent<Item>().item.itIncreaseHealth);
                        }

                        PlayerStat.LocalPlayer.GetEffect(go.GetComponent<Item>().item.itEffect, go.GetComponent<Item>().item.itApplyPercentage / 100);

                        break;

                    case ItemData.ItemEffect.IncreaseMovementSpeed:
                        //??????????????? hp?????? ?????? ????????? ?????????
                        if (go.GetComponent<Item>().item.itIncreaseHealth != 0)
                        {
                            PlayerStat.LocalPlayer.ChangeHp(go.GetComponent<Item>().item.itIncreaseHealth);
                        }

                        PlayerStat.LocalPlayer.GetEffect(go.GetComponent<Item>().item.itEffect, go.GetComponent<Item>().item.itApplyPercentage / 100);

                        break;
                }
            }
            else
            {
                //??????????????? hp?????? ?????? ????????? ?????????
                if (go.GetComponent<Item>().item.itIncreaseHealth != 0)
                {
                    PlayerStat.LocalPlayer.ChangeHp(go.GetComponent<Item>().item.itIncreaseHealth);
                }
            }
        }


        itemList.Remove(go);

        updateBagInventory();

        Destroy(go);
    }
}
