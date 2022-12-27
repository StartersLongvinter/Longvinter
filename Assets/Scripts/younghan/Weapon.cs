using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPun
{
    public enum Type { OneHandMelee, TwoHandMelee, OneHandRange, TwoHandRange }

    [Header("Common")]
    public Type type;
    public float damage;

    [Header("Melee")]
    [SerializeField] private BoxCollider meleeAttackArea;

    [Header("Range")]
    [SerializeField][Range(0, 100)] private int accuracy = 100;
    [SerializeField] private int bulletCountPerFire = 1;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform muzzleFlashPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject muzzleFlashVfxPrefab;
    [SerializeField] private BoxCollider notFireArea;

    public void Fire()
    {
        if (bulletPrefab != null)
        {
            for (int i = 0; i < bulletCountPerFire; i++)
            {
                var bulletInstance = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

                Vector3 bulletDirectionOffset = Vector3.zero;
                if (accuracy != 100)
                {
                    float errorRate = 1 - (accuracy / 100f);

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

                if (photonView.IsMine)
                    bulletInstance.GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, firePoint.forward.x, firePoint.forward.y, firePoint.forward.z, this.damage, bulletDirectionOffset.x, bulletDirectionOffset.y, bulletDirectionOffset.z);
            }
        }

        // if (muzzleFlashVfxPrefab != null)
        // {
        //     GameObject muzzleFlashVfxInstance = Instantiate(muzzleFlashVfxPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
        //     ParticleSystem muzzleFlashParticle = muzzleFlashVfxInstance.GetComponent<ParticleSystem>();

        //     if (muzzleFlashParticle != null)
        //     {
        //         Destroy(muzzleFlashVfxInstance, muzzleFlashParticle.main.duration);
        //     }
        //     else
        //     {
        //         ParticleSystem muzzleFlashParticleChild = muzzleFlashVfxInstance.transform.GetChild(0).GetComponent<ParticleSystem>();

        //         Destroy(muzzleFlashVfxInstance, muzzleFlashParticleChild.main.duration);
        //     }
        // }
        if (muzzleFlashVfxPrefab != null)
        {
            var muzzleFlashVfxInstance = PhotonNetwork.Instantiate(muzzleFlashVfxPrefab.name, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
        }
    }

    public void Slash()
    {
        StartCoroutine(ActivateAttackArea(0.1f, 0.1f));
    }

    public void Saw()
    {
        StartCoroutine(ActivateAttackArea(0f, 0.05f));
    }

    IEnumerator ActivateAttackArea(float onTime, float offTime)
    {
        yield return new WaitForSeconds(onTime);

        meleeAttackArea.enabled = true;

        yield return new WaitForSeconds(offTime);

        meleeAttackArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IDamageable>() == null || (type != Type.OneHandMelee && type != Type.TwoHandMelee))
            return;

        other.gameObject.GetComponent<IDamageable>().ApplyDamage(damage);
    }
}
