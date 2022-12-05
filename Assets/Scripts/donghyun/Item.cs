using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData item;
    public EquipmentData equipment;
    public int weaponIndex;

    public void CallDestroyGameObject()
    {
        GetComponent<PhotonView>().RPC("DestroyGameObject", RpcTarget.All);
    }

    [PunRPC]
    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
