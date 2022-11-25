using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController1 : Turret
{
    private List<GameObject> playerList = new List<GameObject>();
    private int damage = 10;

    private void Update()
    {
        if (playerList.Count==0)
            return;
        if (fireTimeLimit <= 0f&&!isfire)
        {
            isfire=true;
            fireTimeLimit = 1f / fireRate;
        }
        fireTimeLimit -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerList.Add(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (isfire)
        {
            foreach (var i in playerList)
            {
                Enemy enemy = i.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
            }
            isfire=false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerList.Remove(other.gameObject);
    }
}
