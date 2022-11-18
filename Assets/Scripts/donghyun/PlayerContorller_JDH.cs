using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerContorller_JDH : MonoBehaviour
{
    private bool isPressedSpace;
    private bool isClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPressedSpace = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isClicked = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Item")
        {
            if (isPressedSpace)
            {
                isPressedSpace = false;
                if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM)
                {
                    Debug.Log(other.gameObject.name);
                    GetComponent<Encyclopedia>().itemData = other.GetComponent<Item>().item;
                    GetComponent<Encyclopedia>().GainItem();
                    GetComponent<PlayerInventory>().itemList.Add(other.GetComponent<Item>().item);
                    Destroy(other.gameObject);
                }
                else
                {
                    Debug.Log("인벤토리 가득참");
                }
            }

            if (isClicked)
            {
                isClicked = false;
                RaycastHit[] hits;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                hits = Physics.RaycastAll(ray);
                
                var distinctHits = hits.DistinctBy(x => x.collider.name);
                
                foreach (var hit in distinctHits)
                {
                    if (hit.collider.tag == "Item")
                    {
                        if (GetComponent<PlayerInventory>().itemList.Count <= GetComponent<PlayerInventory>().MAXITEM)
                        {
                            Debug.Log(other.gameObject.name);
                            GetComponent<Encyclopedia>().itemData = other.GetComponent<Item>().item;
                            GetComponent<Encyclopedia>().GainItem();
                            GetComponent<PlayerInventory>().itemList.Add(other.GetComponent<Item>().item);
                            Destroy(other.gameObject);
                        }
                    }
                }
            }
        }
    }
}
