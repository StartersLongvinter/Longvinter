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
        instance = this;
    }

    [SerializeField] GameObject inventoryScreen;
    bool pressTab;
    public bool inventoryState;

    //GraphicRaycaster graphicRaycaster;
    //PointerEventData pointerEventData;
    //EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        inventoryState = false;
        //graphicRaycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();

        Open(inventoryScreen);

        //Exitpoint();
    }

    void KeyInput()
    {
        pressTab = Input.GetKeyDown(KeyCode.Tab);
    }

    void Open(GameObject ui)
    {
        if (pressTab)
            inventoryState = !inventoryState;

        ui.SetActive(inventoryState);
    }

    //void Exitpoint()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        pointerEventData = new PointerEventData(eventSystem);

    //        pointerEventData.position = Input.mousePosition;

    //        List<RaycastResult> results = new List<RaycastResult>();

    //        graphicRaycaster.Raycast(pointerEventData, results);

    //        //if (results[0].gameObject.layer == 6)
    //        //{
    //        //    inventoryScreen.SetActive(false);
    //        //}
    //        foreach (RaycastResult result in results)
    //        {
    //            if (result.gameObject.layer == 6)
    //            {
    //                inventoryScreen.SetActive(false);
    //            }
    //        }
    //    }
    //}
}
