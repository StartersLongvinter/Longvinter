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

	public static Color originalColor;
	private SkinnedMeshRenderer renderer;
	public bool isChanged = false;

	void Start ()
	{
		health = startHealth;
	}

	public void TakeDamage (float amount)
	{
		renderer = GetComponentInChildren<SkinnedMeshRenderer>();

		StartCoroutine(ResetColor());

		health -= amount;

		if (health <= 0 && !isDead)
			Die();
	}

	void Die ()
	{
		isDead = true;
		Debug.Log("Dead");

		//Destroy(gameObject);
	}

	IEnumerator ResetColor()
	{
		isChanged = true;
		renderer.material.color = Color.white;
		yield return new WaitForSeconds(0.05f);
		renderer.material.color = Color.red;
		yield return new WaitForSeconds(0.08f);
		renderer.material.color = originalColor;
		yield return null;
		isChanged = false;
	}
}
