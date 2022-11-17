using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour
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
    [SerializeField] private Button inventoryCloseBtn;
    
    private bool isTabPressed;
    private bool inventoryState;
 
    void Start()
    {
        inventoryCloseBtn.onClick.AddListener((() =>
        {
            inventoryState = false;

            bagInventory.SetActive(false);
            equipmentInventory.SetActive(false);
            encyclopediaInventory.SetActive(false);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();

        Open(bagInventory);

        CloseInventory();
    }

    void KeyInput()
    {
        isTabPressed = Input.GetKeyDown(KeyCode.Tab);
    }

    void Open(GameObject ui)
    {
        if (isTabPressed)
        {
            isTabPressed = false;
            inventoryState = !inventoryState;
            ui.SetActive(inventoryState);
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

            }
        }
    }
}