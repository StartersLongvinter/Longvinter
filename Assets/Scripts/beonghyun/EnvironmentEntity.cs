using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnvironmentEntity : MonoBehaviourPun, IDamageable
{
    [SerializeField] string itemName1;
    [SerializeField] string itemName2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage)
    {

    }    

    public void DropItem()
    {

    }
}
