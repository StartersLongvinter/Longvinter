using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HitCheck : MonoBehaviourPunCallbacks
{
    Bear bear;
    private void Start()
    {
        bear = GetComponentInParent<Bear>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            other.gameObject.GetComponent<IDamageable>().ApplyDamage(bear.damage);
        }
    }
}
