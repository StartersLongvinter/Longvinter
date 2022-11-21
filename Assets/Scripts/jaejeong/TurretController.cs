using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private string enemyTag = "Enemy";//!pv.IsMine
    private float range = 30f;
    private Transform target;
    private Enemy targetEnemy;
    public bool useLaser=false;
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float fireTimeLimit = 0f;

    public int damage = 10;
    public float slowAmount = 0.5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;

    public Transform rotatePart;
    public float turnSpeed = 10f;
    public Transform firePoint;

    private void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }
    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach(GameObject enmey in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enmey.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance= distanceToEnemy;
                nearestEnemy = enmey;
            }
        }
        if(nearestEnemy!=null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        }
        else
        {
            target = null;
        }
    }

    private void Update()
    {
        if (target == null)
        {
            if (useLaser)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                }
            }return;
        }
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
        Vector3 dir = target.position - firePoint.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(firePoint.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        rotatePart.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot()
    {
        GameObject firedBullet = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = firedBullet.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Seek(target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        /*Gizmos.DrawLine(target.position , firePoint.position);*/
    }
}
