using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float Damage
	{
		set { damage = value; }
	}

	private Rigidbody bulletRigidbody;
	[SerializeField] private GameObject ImpactVfxPrefab;
	[SerializeField] private float speed;
	private Vector3 direction;
	[SerializeField] private float damage; // SerializeFiled for check, Don't set
	private bool isCollided;

	private void Start()
	{
		bulletRigidbody = GetComponent<Rigidbody>();

		direction = GameObject.FindGameObjectWithTag("Player").transform.forward;
	}

	private void FixedUpdate()
	{
		bulletRigidbody.velocity = direction * speed;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag != "Bullet" && !isCollided)
		{
			isCollided = true;
			speed = 0;
			bulletRigidbody.isKinematic = true;

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
