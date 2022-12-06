using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet_BH : MonoBehaviourPun
{
    public float Damage { set { damage = value; } }
    public Vector3 Direction { set { direction = value; } }

    protected Rigidbody bulletRigidbody;
    [SerializeField] protected float damage;

    [SerializeField] private GameObject ImpactVfxPrefab;
    [SerializeField] private float speed;
    private Vector3 direction;
    private bool isCollided;

    Player hitplayer;

    protected virtual void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();

        transform.rotation = Quaternion.LookRotation(direction);

        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        bulletRigidbody.velocity = direction * speed;
    }

    [PunRPC]
    void Shoot(float x, float y, float z, float _damage = 10f, float _offsetX = 0f, float _offsetY = 0f, float _offsetZ = 0f)
    {
        Bullet_BH bullet = GetComponent<Bullet_BH>();
        this.gameObject.name = photonView.Owner.NickName + "Bullet";
        Vector3 firePoint = new Vector3((float)x, (float)y, (float)z);
        bullet.direction = firePoint + new Vector3(_offsetX, _offsetY, _offsetZ);
        bullet.damage = _damage;

        Debug.Log(bullet.name);
    }

    [PunRPC]
    void HasDamage(int actorNumber)
    {
        Transform enemy = PlayerList.Instance.playersWithActorNumber[actorNumber].transform;
        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null && enemy.GetComponent<PhotonView>().IsMine) ;
            //enemy.GetComponent<PhotonView>().RPC(nameof(e.ChangePlayersColor), RpcTarget.All, damage);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LivingEntity" || collision.gameObject.tag=="Bison")
        {
            Debug.Log("Collided");
            collision.gameObject.GetComponent<LivingEntity>().HitByPlayer(damage);
        }

        if (collision.gameObject.tag == "Player" && (collision.gameObject.name + "Bullet" != this.gameObject.name))
        {
            if (collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                this.GetComponent<PhotonView>().RPC("HasDamage", RpcTarget.All, collision.gameObject.GetComponent<PhotonView>().Owner.ActorNumber);
            }
        }

        if (collision.gameObject.tag != "Bullet" && !isCollided)
        {
            //hitplayer = collision.gameObject.GetComponent<PhotonView>().Owner;
            isCollided = true;
            speed = 0;
            GetComponent<Rigidbody>().isKinematic = true;

            ContactPoint contactPoint = collision.contacts[0];
            Vector3 impactPosition = contactPoint.point;
            Quaternion impactRotation = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);

            if (ImpactVfxPrefab != null)
            {
                GameObject ImpactVfxInstance = Instantiate(ImpactVfxPrefab, impactPosition, impactRotation);
                ParticleSystem impactParticle = ImpactVfxInstance.GetComponent<ParticleSystem>();

                if (impactParticle != null)
                {
                    Destroy(ImpactVfxInstance, impactParticle.main.duration);
                }
                else
                {
                    ParticleSystem impactParticleChild = ImpactVfxInstance.transform.GetChild(0).GetComponent<ParticleSystem>();

                    Destroy(ImpactVfxInstance, impactParticleChild.main.duration);
                }
            }

            //if (shotSFX != null && GetComponent<AudioSource>())
            //{
            //	GetComponent<AudioSource>().PlayOneShot(hitSFX);
            //}

            StartCoroutine(DestroyParticle(0f));
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> transformList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                transformList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);

                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);

                for (int i = 0; i < transformList.Count; i++)
                {
                    transformList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }
}
