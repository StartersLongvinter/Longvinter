using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HitCheck : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            //pv = other.GetComponent<PhotonView>();
            other.gameObject.GetComponent<IDamageable>().ApplyDamage(10);
            //Debug.Log()
        }
    }
}
