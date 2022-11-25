using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingPoint : MonoBehaviourPun
{
    public GameObject[] fishList;

    public bool isOccupied;

    public GameObject SelectRandomFish()
    {
        int fishNumber = Random.Range(0, fishList.Length);
        
        GetComponent<PhotonView>().RPC("UsePoint", RpcTarget.All);

        return fishList[fishNumber];
    }

    public void IsFinished()
    {
        GetComponent<PhotonView>().RPC("NotUsePoint", RpcTarget.All);
    }

    [PunRPC]
    public void UsePoint()
    {
        isOccupied = true;
    }

    [PunRPC]
    public void NotUsePoint()
    {
        isOccupied = false;
    }
}
