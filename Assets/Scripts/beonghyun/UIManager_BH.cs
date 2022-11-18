using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager_BH : MonoBehaviour
{
    public static UIManager_BH instance;

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

    //Fishing
    [SerializeField] GameObject pressEImage;
    [SerializeField] GameObject successImage;
    [SerializeField] TMP_Text fishName;
    [SerializeField] Image fishImage;
    PlayerFishing player;
    Fish fish;

    void Start()
    {
        player = GameObject.Find("Player_BH").GetComponent<PlayerFishing>();

        inventoryCloseBtn.onClick.AddListener((() =>
        {
            inventoryState = false;

            bagInventory.SetActive(false);
            equipmentInventory.SetActive(false);
            encyclopediaInventory.SetActive(false);
            pressEImage.SetActive(false);
            successImage.SetActive(false);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();

        Open(bagInventory);
        OpenEImage();
        //OpenSuccessImage();

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

    void OpenEImage()
    {
        pressEImage.SetActive(player.eImageActivate);
    }

    //물고기 이름과 스프라이트는 fishnumber라는 random int 값을 넣어서 결정
    public void OpenSuccessImage()
    {
        successImage.SetActive(player.isSuccessState);
        fish = player.fish;

        int fishNumber = Random.Range(0, fish.fishList.Length);
        Debug.Log("Success! Caught " + fish.fishList[fishNumber]);
        fishName.text = fish.fishList[fishNumber];
        fishImage.sprite = fish.fishSpriteList[fishNumber];
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
