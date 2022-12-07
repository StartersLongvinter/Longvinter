using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingPoint : MonoBehaviourPun
{
    public GameObject[] fishList;

    public bool isWait;

    int fishCount;

    GameObject effect;

    SphereCollider sphereCollider;

    private void Start()
    {
        fishCount = Random.Range(1, 10);
        effect = transform.GetChild(0).gameObject;
        sphereCollider = GetComponent<SphereCollider>();
    }

    

    private void Update()
    {
        
    }

    public GameObject SelectRandomFish()
    {
        int fishNumber = Random.Range(0, fishList.Length);
        
        //GetComponent<PhotonView>().RPC("UsePoint", RpcTarget.All);

        return fishList[fishNumber];
    }

    public void IsFinished()
    {
        Debug.Log("use");
        GetComponent<PhotonView>().RPC("UsePoint", RpcTarget.All);
    }

    [PunRPC]
    public void UsePoint()
    {
        Debug.Log("usepoint");
        fishCount--;

        if (fishCount==0)
        {
            //gameObject.SetActive(false);
            StartCoroutine(PointSwitcher());
        }
    }
    //public void IsWait()
    //{
    //    GetComponent<PhotonView>().RPC("WaitPoint", RpcTarget.All);
    //}

    //[PunRPC]
    public void WaitPoint()
    {
        Debug.Log("Fish are surprised by sudden movement. Cannot use fishingpoint");
        StartCoroutine(Wait());
    }


    IEnumerator PointSwitcher()
    {
        Debug.Log("coroutine start");

        sphereCollider.enabled = false;
        effect.SetActive(false);

        yield return new WaitForSeconds(Random.Range(1,5));

        sphereCollider.enabled = true;
        effect.SetActive(true);

        fishCount = Random.Range(1,10);
    }

    IEnumerator Wait()
    {
        //sphereCollider.enabled = false;
        isWait = true;

        yield return new WaitForSeconds(10);

        //sphereCollider.enabled = true;
        isWait = false;
    }
    //[PunRPC]
    //public void NotUsePoint()
    //{
    //    isOccupied = false;
    //}
}
