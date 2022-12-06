using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController1 : Turret
{
    private List<GameObject> playerList = new List<GameObject>();
    private float damage = 10;

    protected override void Start()
    {
        base.Start();
        turretOwner = "";
        //photonView.RPC(nameof(TurretController.RepeatInvoke), RpcTarget.All);
    }

    private void Update()
    {
        if (playerList.Count == 0)
            return;
        if (fireTimeLimit <= 0f && !isfire)
        {
            isfire = true;
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
                                    Debug.Log("?");
                                }*/
                if (i.GetComponent<PlayerController>().IsAiming && i.GetComponent<PhotonView>().IsMine)
                {
                    //Enemy enemy = i.GetComponent<Enemy>();
                    PlayerStat playerStat = i.GetComponent<PlayerStat>();
                    i.GetComponent<PhotonView>().RPC(nameof(playerStat.ChangePlayersColor), RpcTarget.All, damage);
                }
            }
            isfire =false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerList.Remove(other.gameObject);
    }
}
