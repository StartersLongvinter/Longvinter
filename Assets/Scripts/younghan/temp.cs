using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour, IDamageable
{
    public float hp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float dmg)
    {
        hp -= dmg;

        Debug.Log(dmg + "   " + hp);
    }

    public void DropItem()
    {

    }
}
