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

    [SerializeField]
    private bool inOtherHome;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }
    private void UpdateTarget() //Turret����
    {
        if (firePoint == null)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            if (PhotonNetwork.MasterClient.ActorNumber == player.GetComponent<PlayerStat>().ownerPlayerActorNumber)
                break;
            float distanceToPlayer = Vector3.Distance(turretTransform.transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }
        if (nearestPlayer != null && shortestDistance <= range && !nearestPlayer.GetComponent<Enemy>().isChanged) //���� ���x, ��ž ��Ÿ� ��, ���� ��� x
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

        if (!inOtherHome && !attack)
            return;

        LockOnTarget();

        if (fireTimeLimit <= 0f && inOtherHome || fireTimeLimit <= 0f&&attack&&!inOtherHome)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            inOtherHome = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            inOtherHome = false;
    }
}
//��ž ��Ÿ� �� && ���� �� ������ ���� ��� ���� ���� ����
//�� �� �� && ��ž ��Ÿ� �� && ���� ��� ���� ����
//Turret ���� �ȿ��� �Ѿȵ�� ����x