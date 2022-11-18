using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    float damageAmount;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
    }

    // Update is called once per frame
    void Update()
    {
        Hit();

        if (currentHealth<=0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    void Hit()
    {
        currentHealth -= damageAmount;
    }
}
