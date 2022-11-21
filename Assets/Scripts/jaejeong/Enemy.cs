using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[HideInInspector]
	public float speed;

	public float startHealth = 100;
	private float health;

	public GameObject deathEffect;

	private bool isDead = false;

	void Start ()
	{
		health = startHealth;
	}

	public void TakeDamage (float amount)
	{
		health -= amount;

		if (health <= 0 && !isDead)
		{
			Die();
		}
	}

	void Die ()
	{
		isDead = true;
		Debug.Log("Dead");

		//Destroy(gameObject);
	}
}
