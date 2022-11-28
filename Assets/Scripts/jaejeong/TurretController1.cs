using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController1 : Turret
{
    private List<GameObject> playerList = new List<GameObject>();
    private float damage = 10;

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
                if (!PlayerList.Instance.playerCharacters.Contains(i))
                {
                    playerList.Remove(i);
                    return;
                }
                if (i.GetComponent<PlayerController>().IsAiming)
                {
                    Enemy enemy = i.GetComponent<Enemy>();
                    //enemy.TakeDamage(damage);
                    i.GetComponent<PhotonView>().RPC(nameof(enemy.TakeDamage), RpcTarget.All, damage);
                }
            }
            isfire=false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerList.Remove(other.gameObject);
    }
}
