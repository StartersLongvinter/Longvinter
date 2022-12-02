using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    void Awake()
    {
        ParticleSystem muzzleFlashParticle = GetComponent<ParticleSystem>();
        if (muzzleFlashParticle != null)
        {
            Destroy(this.gameObject, muzzleFlashParticle.main.duration);
        }
        else
        {
            ParticleSystem muzzleFlashParticleChild = transform.GetChild(0).GetComponent<ParticleSystem>();

            Destroy(this.gameObject, muzzleFlashParticleChild.main.duration);
        }
    }

    void Update()
    {

    }
}
