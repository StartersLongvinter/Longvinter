using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBagInventory : MonoBehaviour
{
    [SerializeField] private GameObject[] rightDeadBagInventory;
    [SerializeField] private GameObject[] leftDeadBagInventory;

    List<GameObject> currentItemList = new List<GameObject>();
    //PlayerInventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

      
    }

    private void OnTriggerStay(Collider other)
    {
        //inventory = other.gameObject.GetComponent<PlayerInventory>();
    }

    //�����Ǹ� ���� �ִ� playerinventory�� itemlist�� ������ �װŴ�� for�� ���� �ڽĿ� instantiate ��������
    private void OnEnable()
    {
        currentItemList = transform.root.gameObject.GetComponent<PlayerInventory>().itemList;

        for (int i = 0; i < currentItemList.Count; i++)
        {
            Instantiate(currentItemList[i], rightDeadBagInventory[i].transform);
        }
    }
}
