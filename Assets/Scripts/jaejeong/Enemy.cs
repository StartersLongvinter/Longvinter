using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
	[HideInInspector]
	public float speed;

	public static Color originalColor;
	private SkinnedMeshRenderer renderer;
	public bool isChanged = false;

    [PunRPC]
	public void TakeDamage (float amount)
	{
		if (!photonView.IsMine)
			return;
		renderer = GetComponentInChildren<SkinnedMeshRenderer>();

		StartCoroutine(ResetColor());

		PlayerStat.LocalPlayer.hp -= amount;

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
		if (renderer.material.color != Color.red) {
			originalColor = renderer.material.color;
			isChanged = true;
			renderer.material.color = Color.red;
			yield return new WaitForSeconds(0.08f);
			renderer.material.color = originalColor;
			isChanged = false;
		}
		yield return null;
	}
}
