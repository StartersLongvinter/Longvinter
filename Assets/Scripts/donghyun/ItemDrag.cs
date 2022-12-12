using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IPointerClickHandler
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private Transform previousPos;
    private Canvas canvas;

    public GameObject[] bagInven;
    public GameObject[] equipInven;

    private GameObject player;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GameObject.Find("Inventory Canvas").GetComponent<Canvas>();
    }

    private void Start()
    {
        //플레이어 이름이 변경되면 반드시 바꿔줘야함.
        player = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);

        bagInven = new GameObject[canvas.transform.GetChild(1).GetChild(0).GetChild(0).childCount];
        equipInven = new GameObject[canvas.transform.GetChild(2).GetChild(0).GetChild(0).childCount];

        for (int i = 0; i < canvas.transform.GetChild(1).GetChild(0).GetChild(0).childCount; i++)
        {
            bagInven[i] = canvas.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).gameObject;
            equipInven[i] = canvas.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(i).gameObject;
        }
    }

    private void InputEquipItem(GameObject item)
    {
        GameObject itemInEquip;

        //장비탭에 현재 넣으려는 장비와 같은 분류를 가지는 장비가 있는지 확인
        if (player.GetComponent<PlayerInventory>().equipmentList.Any(
                x => x.GetComponent<Item>().equipment.eqPosition == item.GetComponent<Item>().equipment.eqPosition))
        {
            //장비탭에 현재 가방에서 클릭한 같은 장비 타입의 게임오브젝트를 찾기
            itemInEquip = player.GetComponent<PlayerInventory>()
                .equipmentList.Find(x =>
                    x.GetComponent<Item>().equipment.eqPosition == item.GetComponent<Item>().equipment.eqPosition);


            #region 장비에 같은 타입이 있을 경우 해당 장비를 가방에 넣고 가방에있는 장비를 장비에 넣는 과정

            //장비리스트에서는 빼고 다시 가방리스트에 추가(이때 서로 교환되는 위치를 가지고 있어야 함)
            // int index = player.GetComponent<PlayerInventory>().itemList.IndexOf(item);
            // player.GetComponent<PlayerInventory>().itemList[index] = itemInEquip;
            //
            // index = player.GetComponent<PlayerInventory>().equipmentList.IndexOf(itemInEquip);
            // player.GetComponent<PlayerInventory>().equipmentList[index] = item;

            player.GetComponent<PlayerInventory>().equipmentList.Remove(itemInEquip);
            player.GetComponent<PlayerInventory>().itemList.Add(itemInEquip);

            int _index = gameObject.GetComponent<Item>().equipment.eqIndex;
            player.GetComponent<PhotonView>().RPC("ActiveOffEquipment", RpcTarget.All, -1);
            player.GetComponent<PhotonView>().RPC("SetWeaponData", RpcTarget.All, true, 0);

            for (int i = 0; i < bagInven.Length; i++)
            {
                if (bagInven[i].transform.childCount == 0)
                {
                    gameObject.transform.SetParent(bagInven[i].transform);
                    transform.localScale = new Vector3(1, 1, 1);
                    GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                    GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                    break;
                }
            }

            //장비와 가방 인벤토리를 업데이트해줘야함
            player.GetComponent<PlayerInventory>().updateEquipInventory();
            player.GetComponent<PlayerInventory>().updateBagInventory();

            Debug.Log(gameObject.GetComponent<Item>().equipment.eqKorName + " 장비에서 빠져서 가방으로 이동함");

            #endregion


            #region 가방에서 장비로 넘기는 과정

            //가방리스트에서 빼고 장비리스트에 추가
            player.GetComponent<PlayerInventory>().itemList.Remove(item);
            player.GetComponent<PlayerInventory>().equipmentList.Add(item);

            // player.GetComponent<PlayerController>().weaponData = gameObject.GetComponent<Item>().equipment;
            player.GetComponent<PhotonView>().RPC("SetWeaponData", RpcTarget.All, false, _index);

            for (int i = 0; i < equipInven.Length; i++)
            {
                if (equipInven[i].transform.childCount == 0)
                {
                    gameObject.transform.SetParent(equipInven[i].transform);
                    transform.localScale = new Vector3(1, 1, 1);
                    GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                    GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                    break;
                }
            }

            player.GetComponent<PlayerInventory>().updateBagInventory();
            player.GetComponent<PlayerInventory>().updateEquipInventory();

            Debug.Log(gameObject.GetComponent<Item>().equipment.eqKorName + " 가방에서 빠져서 장비로 이동함");

            #endregion
        }

        //장비탭에 현재 넣으려는 장비와 같은 분류를 가지는 장비가 없다면 그냥 추가하면됨
        else
        {
            //현재 가방에 있으므로 누르면 장비창으로 넘어가야함
            player.GetComponent<PlayerInventory>().itemList.Remove(gameObject);
            player.GetComponent<PlayerInventory>().equipmentList.Add(gameObject);

            int _index = gameObject.GetComponent<Item>().equipment.eqIndex;
            player.GetComponent<PhotonView>().RPC("SetWeaponData", RpcTarget.All, false, _index);

            for (int i = 0; i < equipInven.Length; i++)
            {
                if (equipInven[i].transform.childCount == 0)
                {
                    gameObject.transform.SetParent(equipInven[i].transform);
                    transform.localScale = new Vector3(1, 1, 1);
                    GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                    GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                    break;
                }
            }

            player.GetComponent<PlayerInventory>().updateBagInventory();

            Debug.Log(gameObject.GetComponent<Item>().equipment.eqKorName + "사용함");
        }
    }

    private void InputBag(GameObject go)
    {
        //장비창에서 가방으로 넘기는 과정
        for (int i = 0; i < bagInven.Length; i++)
        {
            if (bagInven[i].transform.childCount == 0)
            {
                go.transform.SetParent(bagInven[i].transform);
                transform.localScale = new Vector3(1, 1, 1);
                GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(45, 45, 0);
                break;
            }
        }

        player.GetComponent<PlayerInventory>().equipmentList.Remove(go);
        player.GetComponent<PlayerInventory>().itemList.Add(go);

        int _index = gameObject.GetComponent<Item>().equipment.eqIndex;
        player.GetComponent<PhotonView>().RPC("ActiveOffEquipment", RpcTarget.All, _index);
        player.GetComponent<PhotonView>().RPC("SetWeaponData", RpcTarget.All, true, 0);

        player.GetComponent<PlayerInventory>().updateBagInventory();
        player.GetComponent<PlayerInventory>().updateEquipInventory();

        Debug.Log(gameObject.GetComponent<Item>().equipment.eqKorName + "사용함");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SoundManager.Instance.PlayToolSound("InventorySounds", 2);
            //장비가 아닌 아이템은 그냥 사용만 하면됨
            if (gameObject.GetComponent<Item>().item != null)
            {
                if (player.GetComponent<PlayerInventory>().currentUseItem.Any(x => x.GetComponent<Item>().item.itEffect
                        == gameObject.GetComponent<Item>().item.itEffect))
                {
                    Debug.Log("현재 적용된 아이템입니다.");

                    return;
                }
                Debug.Log(gameObject.GetComponent<Item>().item.itKorName + "사용함");

                transform.SetParent(canvas.transform);
                transform.SetAsFirstSibling();

                player.GetComponent<PlayerInventory>().ItemUse(gameObject);
            }
            else if (gameObject.GetComponent<Item>().equipment != null)
            {
                //ammo를 인벤토리에서 클릭 했을 경우 장비에 들어가는게 아니라 플레이어의 ammo에 채워져야함
                if (gameObject.GetComponent<Item>().equipment.eqClassify == EquipmentData.EquipmentClassify.Ammo)
                {
                    int currentAmmoCount = int.Parse(player.GetComponent<Ammo>().ammoText.text);

                    currentAmmoCount += gameObject.GetComponent<Item>().equipment.amCount;

                    player.GetComponent<Ammo>().ammoText.text = currentAmmoCount.ToString();
                    player.GetComponent<PlayerInventory>().itemList.Remove(gameObject);

                    transform.SetParent(canvas.transform);

                    player.GetComponent<PlayerInventory>().updateBagInventory();

                    Destroy(gameObject);
                }
                else
                {
                    if (transform.parent.parent.parent.parent.name.Equals("Bag"))
                    {
                        InputEquipItem(gameObject);
                    }
                    else if (transform.parent.parent.parent.parent.name.Equals("Equipment"))
                    {
                        InputBag(gameObject);
                    }
                }
            }
        }
    }
}