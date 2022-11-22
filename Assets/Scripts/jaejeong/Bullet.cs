using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public GameObject impactEffect;

	private Transform target;
	private Vector3 targetPosition;
	private float speed = 100f;
	private int damage = 10;
	private Vector3 dir;


	public void Seek (Transform _target)
	{
		target = _target;
		targetPosition = target.position;
	}

	// Update is called once per frame
	void Update () {

		if (target == null)
		{
			Destroy(gameObject);
			return;
		}

		dir = targetPosition - transform.position;
		float distanceThisFrame = speed * Time.deltaTime;

		transform.Translate(dir.normalized * distanceThisFrame, Space.World);
		transform.LookAt(target);
		Destroy(gameObject, 3f);
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
			e.TakeDamage(damage);
	}

    private void OnCollisionEnter(Collision collision)
    {
		Debug.Log(collision.gameObject.tag);
		if (collision.gameObject.tag == "Player")
		{
			HitTarget();
			return;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
    }
}
