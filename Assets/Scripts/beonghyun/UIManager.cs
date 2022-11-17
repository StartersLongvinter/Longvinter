using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject inventoryScreen;
    [SerializeField] private Button inventoryCloseBtn;

    private bool isTabPressed;
    private bool inventoryState;

    void Start()
    {
        inventoryCloseBtn.onClick.AddListener((() =>
        {
            inventoryState = false;
            inventoryScreen.SetActive(false);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();

        Open(inventoryScreen);

        CloseInventory();
    }

    void KeyInput()
    {
        isTabPressed = Input.GetKeyDown(KeyCode.Tab);
    }

    void Open(GameObject ui)
    {
        if (isTabPressed)
            inventoryState = !inventoryState;

        ui.SetActive(inventoryState);
    }

    void CloseInventory()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                inventoryState = false;
                inventoryScreen.SetActive(false);
            }
        }
    }
}