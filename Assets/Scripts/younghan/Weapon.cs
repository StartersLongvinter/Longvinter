using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Melee1 - Axe / Melee2 - Chain saw / Range - Gun
    public enum Type { Melee1, Melee2, Range }
    //public enum AttackMode { Single, Auto }

    [Header("Common")]
    public Type type;
    //public AttackMode attackMode;
    public float damage;
    public float attackRate;

    [Header("Range")]
    [SerializeField] [Range(0, 100)] private int accuracy = 100;
    [SerializeField] private int bulletCountPerFire = 1;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform muzzleFlashPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject muzzleFlashVfxPrefab;

    public void Fire()
    {
        if (bulletPrefab != null)
        {
            for (int i = 0; i < bulletCountPerFire; i++)
            {
                GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                bulletInstance.name = i.ToString();

                Vector3 bulletDirectionOffset = Vector3.zero;
                if (accuracy != 100)
                {
                    float errorRate = 1 - (accuracy / 100);

                    for (int j = 0; j < 2; j++)
                    {
                        float randomErrorRate = 1 * Random.Range(-errorRate, errorRate);
                        int plusOrMinus = Random.Range(0, 2); // 0 = plus, 1 = minus

                        if (j == 0) // Set x offset
                        {
                            if (plusOrMinus == 0) // (+x, 0, 0)
                                bulletDirectionOffset = new Vector3(randomErrorRate, 0, 0);
                            else // (-x, 0, 0)
                                bulletDirectionOffset = new Vector3(-randomErrorRate, 0, 0);
                        }
                        else // Set z offset
                        {
                            if (plusOrMinus == 0) // (x, 0, +z)
                                bulletDirectionOffset = new Vector3(bulletDirectionOffset.x, 0, randomErrorRate);
                            else // (x, 0, -z)
                                bulletDirectionOffset = new Vector3(bulletDirectionOffset.x, 0, -randomErrorRate);
                        }
                    }
                }

                bulletInstance.GetComponent<Bullet>().Direction = firePoint.forward + bulletDirectionOffset;
                bulletInstance.GetComponent<Bullet>().Damage = this.damage;
            }
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
}
