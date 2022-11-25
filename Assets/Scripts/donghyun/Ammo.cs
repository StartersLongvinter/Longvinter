using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammo : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject itemBag;
    [SerializeField] private Button dropAmmoBtn;
    [SerializeField] private EquipmentData[] ammoData;
    
    public Text ammoText;

    
    
    //ammo 100 > 100, 50 > 50, 25 > 25
    // Start is called before the first frame update
    void Start()
    {
        
        
        dropAmmoBtn.onClick.AddListener((() =>
        {
            int ammo = int.Parse(ammoText.text);
            
            if (ammo >= 100)
            {
                Debug.Log("100개 생성");
                itemBag.GetComponent<Item>().equipment = ammoData[2];

                GameObject item = Instantiate(itemBag, player.transform.position, Quaternion.identity);
                item.tag = "Equipment";
                
                ammo -= 100;
                ammoText.text = ammo.ToString();
            }
            else if (ammo is >= 50 and < 100)
            {
                Debug.Log("50개 생성");
                itemBag.GetComponent<Item>().equipment = ammoData[1];
                
                GameObject item = Instantiate(itemBag, player.transform.position, Quaternion.identity);
                item.tag = "Equipment";
                
                ammo -= 50;
                ammoText.text = ammo.ToString();
            }
            else if (ammo is >= 25 and < 50)
            {
                Debug.Log("25개 생성");
                itemBag.GetComponent<Item>().equipment = ammoData[0];
                
                GameObject item = Instantiate(itemBag, player.transform.position, Quaternion.identity);
                item.tag = "Equipment";
                
                ammo -= 25;
                ammoText.text = ammo.ToString();
            }
            else
            {
                Debug.Log("총알이 부족합니다.");
            }
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
