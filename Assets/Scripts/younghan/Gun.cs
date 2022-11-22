using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    //public GameObject bulletPrefab;

    private void Awake()
    {
        type = Type.Range;
    }

    void Update()
    {
        
    }

    public override void Attack()
    {
        //base.Attack();

        //Fire();
    }

    //private void Fire()
    //{
    //    Debug.Log("Fire");
    //}

}
