using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[HideInInspector]
	public float speed;

	public static Color originalColor;
	private SkinnedMeshRenderer renderer;
	public bool isChanged = false;

	public void TakeDamage (float amount)
	{
		renderer = GetComponentInChildren<SkinnedMeshRenderer>();

		StartCoroutine(ResetColor());

		PlayerStat.LocalPlayer.hp -= amount;
		Debug.Log(PlayerStat.LocalPlayer.hp);

		if (PlayerStat.LocalPlayer.hp <= 0 && PlayerStat.LocalPlayer.status != PlayerStat.Status.die)
			Die();
	}

	void Die ()
	{
		PlayerStat.LocalPlayer.status = PlayerStat.Status.die;
		Debug.Log("Dead");
	}

	IEnumerator ResetColor()
	{
		originalColor = renderer.material.color;
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
