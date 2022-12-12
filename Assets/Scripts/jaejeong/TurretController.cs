using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController : MonoBehaviour, IPunObservable
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform turretTransform;
    public GroundTrigger trigger;

    public Transform rotatePart;

    public float fireRate = 1f;
    public float fireTimeLimit = 0f;
    public bool isfire = false;
    public bool attack;

    public string turretOwner = "";

    public bool IsAuto;
    public bool IsPublic;

    [SerializeField] Material[] mat;
    [SerializeField] private float range = 10f;

    private string playerTag = "Player";
    private Transform target;
    private float damage;

    private void Start()
    {
        StartCoroutine(RotateTurret());
        if (turretOwner == "")
        {
            IsPublic = true;
            damage = 10;
            /*            StartCoroutine(IsMasterClientIn());;*/
            Invoke("GongYongAh", 3f);
            /*GongYongAh()*/
        }
        else
        {
            GetComponent<PhotonView>().RPC("Init", RpcTarget.All);
        }
    }

    void GongYongAh()
    {
        Debug.Log("1");
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("RepeatInvoke", RpcTarget.All);
            Debug.Log("?");
        }
    }

    IEnumerator IsMasterClientIn()
    {
        Debug.Log("¹¹Áö");
        while (!PhotonNetwork.InRoom)
            yield return IsMasterClientIn();
        GongYongAh();
        Debug.Log("2");
        yield break;
    }

    [PunRPC]
    public void Init()
    {
        trigger = GameObject.Find(GetComponent<PhotonView>().Owner.NickName + "HomeArea").GetComponent<GroundTrigger>();
        trigger.myTurret = this;

        if (GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
            GetComponent<PhotonView>().RPC("RepeatInvoke", RpcTarget.All);
    }

    [PunRPC]
    public void RepeatInvoke()
    {
        Debug.Log("3");
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void UpdateTarget()
    {
        Debug.Log("4");
        if (firePoint == null)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            Debug.Log(player);
        }
        foreach (GameObject player in players)
        {
            if (!IsPublic &&GetComponent<PhotonView>().Owner.NickName == player.name ||IsPublic&&!player.GetComponent<PlayerController>().IsAiming || player.GetComponent<PlayerStat>().status == PlayerStat.Status.Die)
            //if(GetComponent<PhotonView>().Owner.NickName == player.name || player.GetComponent<PlayerStat>().status == PlayerStat.Status.Die)
                continue;
            float distanceToPlayer = Vector3.Distance(turretTransform.transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }
        if (shortestDistance > range && !IsPublic)
            trigger.inOtherHome = false;
        if (nearestPlayer != null && shortestDistance <= range && !nearestPlayer.GetComponent<PlayerStat>().isColorChanged)
        {
            GetComponent<PhotonView>().RPC("ChangeTarget", RpcTarget.All, nearestPlayer.GetComponent<PhotonView>().Owner.ActorNumber);
            PlayerStat.playerOriginalColor = nearestPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        }
        else
            target = null;
    }

    private void Update()
    {
/*        if (!IsPublic)
            ChangeTurretModeColor();*/

        if (target == null || firePoint == null)
            return;
        attack = target.GetComponent<PlayerController>().IsAiming;
        if (!attack && (IsPublic || (!trigger.inOtherHome && !IsPublic)))
            return;

        if (GetComponent<PhotonView>().IsMine)
            GetComponent<PhotonView>().RPC("LockOnTarget", RpcTarget.All, target.position.x, rotatePart.position.y, target.position.z);

        if (fireTimeLimit <= 0f && !IsPublic && ((trigger.inOtherHome && (IsAuto || (attack && !IsAuto))) || (!trigger.inOtherHome && attack)))
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                var firedBullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation * Quaternion.Euler(new Vector3(0, 0, 0)));
                firedBullet.GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, -firePoint.right.x, -firePoint.right.y, -firePoint.right.z, 10f, 0f, 0f, 0f);
            }
            fireTimeLimit = 1f / fireRate;
        }else if(fireTimeLimit <= 0f && IsPublic && attack)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && target.GetComponent<PhotonView>().Owner==PhotonNetwork.LocalPlayer)
            {
                PlayerStat playerStat = target.GetComponent<PlayerStat>();
                target.GetComponent<PhotonView>().RPC(nameof(playerStat.ApplyDamage), RpcTarget.All, 10f);
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
            for (float time = 0; time < duration; time += Time.fixedDeltaTime)
            {
                rotatePart.transform.Rotate(Vector3.forward, speed * Time.fixedDeltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ChangeTurretModeColor()
    {
        Material[] materials = this.transform.GetChild(0).GetComponent<MeshRenderer>().materials;

        if (IsAuto)
            materials[0] = mat[1];
        else
            materials[0] = mat[0];
        this.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
    }

    /*public void ChangeTurretMode(bool isChangeMode)
    {
        Transform ColliderTurret = this.transform.GetChild(2);
        if (isChangeMode)
        {
            if (IsAuto)
            {
                ColliderTurret.GetChild(1).gameObject.SetActive(true);
                ColliderTurret.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                ColliderTurret.GetChild(1).gameObject.SetActive(true);
                ColliderTurret.GetChild(4).gameObject.SetActive(true);
            }
        }
        else
        {
            ColliderTurret.GetChild(1).gameObject.SetActive(false);
            ColliderTurret.GetChild(4).gameObject.SetActive(false);
        }
    }*/

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((bool)IsAuto);
            stream.SendNext((string)turretOwner);
        }

        else
        {
            IsAuto = (bool)stream.ReceiveNext();
            turretOwner = (string)stream.ReceiveNext();
            ChangeTurretModeColor();
            if(turretOwner==PhotonNetwork.LocalPlayer.NickName)
                GetComponent<PhotonView>().RequestOwnership();
        }
    }
}