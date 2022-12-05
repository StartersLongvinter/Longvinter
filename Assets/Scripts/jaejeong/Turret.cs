using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Turret : MonoBehaviourPun
{
    public Transform rotatePart;
    public Transform turretTransform;
    public GroundTrigger trigger;

    public float fireRate = 1f;
    public float fireTimeLimit = 0f;
    public bool isfire = false;
    public bool attack;

    public Transform firePoint;
    public Transform target;

    public float range = 10f;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(RotateTurret());
    }

    public IEnumerator RotateTurret()
    {
        float duration = 9f;
        float speed = 5f;
        while (true)
        {
            for (float time = 0; time < duration; time += Time.fixedDeltaTime)
            {
                rotatePart.transform.Rotate(Vector3.forward, speed * Time.fixedDeltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateTargetFunc(GameObject[] players)
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            if (photonView.Owner.NickName == player.name || player.GetComponent<PlayerStat>().status == PlayerStat.Status.die)
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
        if (nearestPlayer != null && shortestDistance <= range && !nearestPlayer.GetComponent<Enemy>().isChanged)
        {
            photonView.RPC(nameof(TurretController.ChangeTarget), RpcTarget.All, nearestPlayer.GetComponent<PhotonView>().Owner.ActorNumber);
            Enemy.originalColor = nearestPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        }
        else
            target = null;
    }
    protected virtual void Update()
    {
        if (target == null || firePoint == null)
            return;

        attack = target.GetComponent<PlayerController>().IsAiming;

        if (!trigger.inOtherHome && !attack)
            return;

        if (photonView.IsMine)
            photonView.RPC(nameof(TurretController.LockOnTarget), RpcTarget.All, target.position.x, rotatePart.position.y, target.position.z);
    }
}
