using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Melee1 - Axe / Melee2 - Chain saw / Range - Gun
    public enum Type { Melee1, Melee2, Range }
    //public enum AttackMode { Single, Auto }

    public Type type;
    //public AttackMode attackMode;
    public float damage;
    public float attackRate;
    public Transform firePoint;
    public Transform muzzleFlashPoint;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashVfxPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Fire()
    {
        if (bulletPrefab != null)
        {
            GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            bulletInstance.GetComponent<Bullet>().Damage = this.damage;
            bulletInstance.GetComponent<Bullet>().Direction = firePoint.forward;
        }

        if (muzzleFlashVfxPrefab != null)
        {
            GameObject muzzleFlashVfxInstance = Instantiate(muzzleFlashVfxPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
            ParticleSystem muzzleFlashParticle = muzzleFlashVfxInstance.GetComponent<ParticleSystem>();

            if (muzzleFlashParticle != null)
            {
                Destroy(muzzleFlashVfxInstance, muzzleFlashParticle.main.duration);
            }
            else
            {
                ParticleSystem muzzleFlashParticleChild = muzzleFlashVfxInstance.transform.GetChild(0).GetComponent<ParticleSystem>();

                Destroy(muzzleFlashVfxInstance, muzzleFlashParticleChild.main.duration);
            }
        }
    }

    public void Swing()
    {
        Debug.Log("Swing");
    }

    public virtual void Attack() { }


}
