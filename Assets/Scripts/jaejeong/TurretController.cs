using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController : Turret
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform turretTransform;

    [SerializeField] private float range = 30f;

    private string playerTag = "Player";

    private Transform target;
    private Enemy targetEnemy;

    public GroundTrigger trigger;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }
    private void UpdateTarget() //Turret범위
    {
        if (firePoint == null)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(turretTransform.transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }
        if (shortestDistance > range)
            trigger.inOtherHome = false;
        if (nearestPlayer != null && shortestDistance <= range && !nearestPlayer.GetComponent<Enemy>().isChanged) //집터 상관x, 포탑 사거리 안, 무기 상관 x
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
        if (target == null||firePoint==null)
            return;

        attack = target.GetComponent<PlayerController>().IsAiming;

        Debug.Log(trigger.inOtherHome + " " + attack);

        if (!trigger.inOtherHome && !attack)
            return;

        LockOnTarget();

        if (fireTimeLimit <= 0f && trigger.inOtherHome || fireTimeLimit <= 0f&&attack&&!trigger.inOtherHome)
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
        GameObject firedBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(new Vector3(0, 90, 0)));
        TurretBullet bullet = firedBullet.GetComponent<TurretBullet>();

        bullet.Direction = firePoint.right;
        
        if (bullet != null)
            bullet.Seek(target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
//포탑 사거리 안 && 집터 에 있으면 무기 상관 없이 공격 가능
//집 터 밖 && 포탑 사거리 안 && 무기 들면 공격 가능
//Turret 범위 안에서 총안들면 공격x