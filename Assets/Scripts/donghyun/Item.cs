using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData Fish;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Fish.itType == ItemData.ItemType.Food && 
                other.GetComponent<PlayerInventory>().itemList.Count <= other.GetComponent<PlayerInventory>().MAXITEM)
            {
                other.GetComponent<PlayerInventory>().itemList.Add(Fish);
            }
        }
    }
}
