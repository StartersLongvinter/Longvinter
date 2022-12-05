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
        base.Update();
        if (playerList.Count == 0)
            return;
        if (fireTimeLimit <= 0f && !isfire)
        {
            isfire = true;
            Enemy enemy=target.GetComponent<Enemy>();
            target.GetComponent<PhotonView>().RPC(nameof(enemy.ChangePlayersColor), RpcTarget.All, damage);

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
                    continue;
                }
                /*                if (i.GetComponent<PlayerController>().IsAiming)
                                {
                                    Enemy enemy = i.GetComponent<Enemy>();
                                    if (i.GetComponent<PhotonView>().IsMine)
                                        i.GetComponent<PhotonView>().RPC(nameof(enemy.ChangePlayersColor), RpcTarget.All, damage);
                                }*/
                /*                if (i.GetComponent<PlayerController>().IsAiming && i.GetComponent<PhotonView>().IsMine)
                                {
                                    Enemy enemy = i.GetComponent<Enemy>();
                                    i.GetComponent<PhotonView>().RPC(nameof(enemy.ChangePlayersColor), RpcTarget.All, damage);
                                }*/
            }
            UpdateTargetFunc(playerList.ToArray());
            isfire =false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerList.Remove(other.gameObject);
    }
}
