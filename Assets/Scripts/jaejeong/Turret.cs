using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Turret : MonoBehaviourPun
{
    public Transform rotatePart;

    public float fireRate = 1f;
    public float fireTimeLimit = 0f;
    public bool isfire = false;
    public bool attack;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //StartCoroutine(RotateTurret());
    }

    public IEnumerator RotateTurret()
    {
        float duration = 9f;
        float speed = 5f;
        while (true)
        {
            for (float time = 0; time < duration; time += Time.fixedDeltaTime)
            {
                rotatePart.transform.Rotate(Vector3.up, speed * Time.fixedDeltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
