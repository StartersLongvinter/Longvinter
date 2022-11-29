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

    int fishCount;

    GameObject effect;

    SphereCollider sphereCollider;

    private void Start()
    {
        fishCount = /*Random.Range(1, 10)*/1;
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

    //[PunRPC]
    //public void ActivatePoint()
    //{
    //    gameObject.SetActive(true);
    //}

    IEnumerator PointSwitcher()
    {
        Debug.Log("coroutine start");

        sphereCollider.enabled = false;
        effect.SetActive(false);

        yield return new WaitForSeconds(Random.Range(1,5));

        sphereCollider.enabled = true;
        effect.SetActive(true);

        fishCount = 2;
    }

    //[PunRPC]
    //public void NotUsePoint()
    //{
    //    isOccupied = false;
    //}
}
