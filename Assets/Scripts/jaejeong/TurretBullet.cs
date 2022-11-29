using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretBullet : Bullet
{
	//public GameObject impactEffect;

	private Transform target;
	private Vector3 targetPosition;

    //[PunRPC]
    public void Seek(Transform _target)
    {
		Debug.Log(target.name+"¤Ì¤Ì");
		target = _target; 
		targetPosition = target.position;
    }

    protected override void Start()
    {
		bulletRigidbody = GetComponent<Rigidbody>();
	}

/*    // Update is called once per frame
    void Update () {

		if (target == null&&PhotonNetwork.IsMasterClient)
		{
			//Destroy(gameObject);
			PhotonNetwork.Destroy(gameObject);
			return;
		}
	}*/

	[PunRPC]
	void HitTarget ()
	{
			Damage(target);
		/*if (target.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
			target.GetComponent<PhotonView>().RPC("Damage", RpcTarget.All, target);*/
		//Destroy(gameObject);
		PhotonNetwork.Destroy(gameObject);
	}

    
	void Damage (Transform enemy)
	{
		Enemy e = enemy.GetComponent<Enemy>();
		if (e != null && enemy.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
			enemy.GetComponent<PhotonView>().RPC(nameof(e.ChangePlayersColor), RpcTarget.All, damage);
			//e.TakeDamage(damage);
	}

    protected override void OnCollisionEnter(Collision collision)
    {
		base.OnCollisionEnter(collision);
		if (collision.gameObject.tag == "Player"&&(collision.gameObject.name + "Bullet" !=this.gameObject.name))
		{
			if (target.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
			{
				//HitTarget();
				this.GetComponent<PhotonView>().RPC("HitTarget", RpcTarget.All);
				return;
			}
		}
    }
}
