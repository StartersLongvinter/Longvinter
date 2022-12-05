using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviourPun
{
    public static UIManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private GameObject bagInventory;
    [SerializeField] private GameObject equipmentInventory;
    [SerializeField] private GameObject encyclopediaInventory;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private Button inventoryCloseBtn;
    
    [SerializeField] private GameObject bagToEquipmentBtn;
    [SerializeField] private GameObject bagToEncyclopediaBtn;
    [SerializeField] private GameObject equipmentToBagBtn;
    [SerializeField] private GameObject equipmentToEncyclopediaBtn;

    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject encyclopediaPanel;
    
    private bool isTabPressed;
    private bool inventoryState;
    private int currentOpenInven = 0;

    
    //Fishing
    [SerializeField] GameObject pressEImage;
    [SerializeField] Text fishName;
    [SerializeField] Image fishImage;
    PlayerController player;
 
    void Start()
    {
        player = GetComponent<PlayerController>();
        
        inventoryCloseBtn.onClick.AddListener((() =>
        {
            inventoryState = false;

            bagInventory.SetActive(false);
            equipmentInventory.SetActive(false);
            encyclopediaInventory.SetActive(false);
            pressEImage.SetActive(false);
        }));
        
        bagToEquipmentBtn.GetComponent<Button>().onClick.AddListener((() =>
        {
            bagPanel.SetActive(false);
            equipmentPanel.SetActive(true);

            currentOpenInven = 1;
        }));
        
        bagToEncyclopediaBtn.GetComponent<Button>().onClick.AddListener((() =>
        {
            bagPanel.SetActive(false);
            equipmentPanel.SetActive(false);
            encyclopediaPanel.SetActive(true);

            inventoryState = false;
            currentOpenInven = 0;
        }));
        
        equipmentToBagBtn.GetComponent<Button>().onClick.AddListener((() =>
        {
            bagPanel.SetActive(true);
            equipmentPanel.SetActive(false);

            currentOpenInven = 0;
        }));
        
        equipmentToEncyclopediaBtn.GetComponent<Button>().onClick.AddListener((() =>
        {
            bagPanel.SetActive(false);
            equipmentPanel.SetActive(false);
            encyclopediaPanel.SetActive(true);

            inventoryState = false;
            currentOpenInven = 1;
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        
        KeyInput();

        Open(bagInventory, equipmentInventory);
        OpenEImage();
        
        CloseInventory();
    }

    void KeyInput()
    {
        isTabPressed = Input.GetKeyDown(KeyCode.Tab);
    }

    void Open(GameObject bag, GameObject equipment)
    {
        if (isTabPressed)
        {
            isTabPressed = false;

            if (!encyclopediaPanel.activeSelf)
            {
                if (inventoryState)
                {
                    bag.SetActive(false);
                    equipment.SetActive(false);
                    toolTip.SetActive(false);
                }
                else
                {
                    switch (currentOpenInven)
                    {
                        case 0:
                            bagPanel.SetActive(true);
                            break;
            
                        case 1:
                            equipmentPanel.SetActive(true);
                            break;
                    }
                }
            }
            else
            {
                encyclopediaPanel.SetActive(false);
                
                if (inventoryState)
                {
                    bag.SetActive(false);
                    equipment.SetActive(false);
                    toolTip.SetActive(false);
                }
                else
                {
                    switch (currentOpenInven)
                    {
                        case 0:
                            bagPanel.SetActive(true);
                            break;
            
                        case 1:
                            equipmentPanel.SetActive(true);
                            break;
                    }
                }
            }
            
            inventoryState = !inventoryState;
        }
    }

    private void CheckInventoryState()
    {
        
    }
    
    void OpenEImage()
    {
        pressEImage.SetActive(player.eImageActivate);
    }
    
    //������ �̸��� ��������Ʈ�� fishnumber��� random int ���� �־ ����
    public void OpenSuccessImage(GameObject fish)
    {
        if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM)
        {
            GetComponent<Encyclopedia>().itemData = fish.GetComponent<Item>().item;
            GetComponent<Encyclopedia>().GainItem();

            GameObject temp = Instantiate(fish);
            
            GetComponent<PlayerInventory>().AddItem(temp);
        }
        else
        {
            Debug.Log("인벤토리 가득참");
        }
    }

    void CloseInventory()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                inventoryState = false;
                bagInventory.SetActive(false);
                equipmentInventory.SetActive(false);
                encyclopediaInventory.SetActive(false);
                toolTip.SetActive(false);
            }
        }
    }
}