using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretBullet : Bullet
{
	//public GameObject impactEffect;

	private Transform target;
	private Vector3 targetPosition;

	public void Seek (Transform _target)
	{
		target = _target;
		targetPosition = target.position;
	}

    protected override void Start()
    {
		bulletRigidbody = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update () {

		if (target == null)
		{
			Destroy(gameObject);
			return;
		}
	}

    void HitTarget ()
	{
		Damage(target);
		Destroy(gameObject);
	}

	void Damage (Transform enemy)
	{
		Enemy e = enemy.GetComponent<Enemy>();
		if (e != null)
			enemy.GetComponent<PhotonView>().RPC(nameof(e.TakeDamage), RpcTarget.All, damage);
		//e.TakeDamage(damage);
	}

    protected override void OnCollisionEnter(Collision collision)
    {
		base.OnCollisionEnter(collision);
		Debug.Log(collision.gameObject.tag);
		if (collision.gameObject.tag == "Player")
		{
			HitTarget();
			return;
		}
    }
}
