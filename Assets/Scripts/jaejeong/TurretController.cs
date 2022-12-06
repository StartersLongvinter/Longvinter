using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController : Turret
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform turretTransform;
    public GroundTrigger trigger;

    [SerializeField] private float range = 10f;

    private string playerTag = "Player";
    //private string turretOwner = "";

    private Transform target;
    private Enemy targetEnemy;

    protected override void Start()
    {
        base.Start();
        /*        if(turretOwner!="")
                    turretOwner = photonView.Owner.NickName;*/

        photonView.RPC("Init", RpcTarget.All);
    }

    [PunRPC]
    public void Init()
    {
        trigger = GameObject.Find(photonView.Owner.NickName + "HomeArea").GetComponent<GroundTrigger>();
        trigger.myTurret = this;

        if (photonView.Owner == PhotonNetwork.LocalPlayer)
            photonView.RPC("RepeatInvoke", RpcTarget.All);
    }

    [PunRPC]
    public void RepeatInvoke()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void UpdateTarget()
    {
        if (firePoint == null)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            if (photonView.Owner.NickName == player.name || player.GetComponent<PlayerStat>().status == PlayerStat.Status.Die)
                continue;
            float distanceToPlayer = Vector3.Distance(turretTransform.transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }
        if (shortestDistance > range)
            trigger.inOtherHome = false;
        if (nearestPlayer != null && shortestDistance <= range && !nearestPlayer.GetComponent<PlayerStat>().isColorChanged)
        {
            photonView.RPC("ChangeTarget", RpcTarget.All, nearestPlayer.GetComponent<PhotonView>().Owner.ActorNumber);
            PlayerStat.playerOriginalColor = nearestPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        }
        else
            target = null;
    }

    private void Update()
    {
        if (target == null || firePoint == null)
            return;

        attack = target.GetComponent<PlayerController>().IsAiming;
        if (!trigger.inOtherHome && !attack)
            return;

        if (photonView.IsMine)
            photonView.RPC("LockOnTarget", RpcTarget.All, target.position.x, rotatePart.position.y, target.position.z);

        //if (fireTimeLimit <= 0f && trigger.inOtherHome&& IsAuto || fireTimeLimit <= 0f && attack && !trigger.inOtherHome)
        if (fireTimeLimit <= 0f && turretOwner != null && (trigger.inOtherHome && (IsAuto || (attack && !IsAuto)) || (!trigger.inOtherHome && attack)))
        {
            if (photonView.IsMine)
            {
                var firedBullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation * Quaternion.Euler(new Vector3(0, 0, 0)));
                firedBullet.GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, -firePoint.right.x, -firePoint.right.y, -firePoint.right.z, 10f, 0f, 0f, 0f);
            }
            fireTimeLimit = 1f / fireRate;
        }
        fireTimeLimit -= Time.deltaTime;
    }

    [PunRPC]
    public void LockOnTarget(float x, float y, float z)
    {
        StopCoroutine(RotateTurret());
        Vector3 position = new Vector3(x, y, z);
        rotatePart.transform.LookAt(position);
        rotatePart.transform.Rotate(-90, 90, 0);
    }

    [PunRPC]
    public void ChangeTarget(int actorNumber)
    {
        target = PlayerList.Instance.playersWithActorNumber[actorNumber].transform;
        targetEnemy = target.GetComponent<Enemy>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}