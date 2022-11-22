using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController : MonoBehaviourPun
{
    public GameObject bulletPrefab;
    public Transform rotatePart;
    public Transform firePoint;

    private string playerTag = "Player";//!pv.IsMine
    private float range = 30f;
    private Transform target;
    private Enemy targetEnemy;
    private float fireRate = 1f;
    private float fireTimeLimit = 0f;

    private void Start()
    {
        StartCoroutine(RotateTurret());
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }
    private void UpdateTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            /*if (PhotonNetwork.MasterClient.ActorNumber == player.GetComponent<PlayerStat>().ownerPlayerActorNumber)
                break;*/
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }
        if (nearestPlayer != null && shortestDistance <= range &&!nearestPlayer.GetComponent<Enemy>().isChanged)
        {
            target = nearestPlayer.transform;
            targetEnemy = nearestPlayer.GetComponent<Enemy>();
            Enemy.originalColor = nearestPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        }
        else
            target = null;
    }

    private void Update()
    {
        if (target == null)
            return;

        LockOnTarget();

        if (fireTimeLimit <= 0f)
        {
            Shoot();
            fireTimeLimit = 1f / fireRate;
        }
        fireTimeLimit -= Time.deltaTime;
    }

    void LockOnTarget()
    {
        StopCoroutine(RotateTurret());
        rotatePart.transform.LookAt(new Vector3(target.position.x, rotatePart.position.y, target.position.z));
        rotatePart.transform.Rotate(0, -90, 0);
    }

    void Shoot()
    {
        GameObject firedBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = firedBullet.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Seek(target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public IEnumerator RotateTurret()
    {
        float duration = 9f;
        float speed = 5f;
        while (true)
        {
            for(float time=0; time<duration; time += Time.fixedDeltaTime)
            {
                rotatePart.transform.Rotate(Vector3.up, speed * Time.fixedDeltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

/*    private void OnTriggerStay(Collider other)
    {
        //집이랑 Turret 범위 겹치면 공격 가능
        //집 범위에서 총들면 공격 가능
    }*/
}