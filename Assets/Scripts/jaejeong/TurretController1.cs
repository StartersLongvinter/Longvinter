using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController1 : MonoBehaviour
{
    public Transform rotatePart;
    //public Transform firePoint;

    private string playerTag = "Player";//!pv.IsMine
    private float range = 30f;
    private Transform target;
    private Enemy targetEnemy;
    private float fireRate = 1f;
    private float fireTimeLimit = 0f;
    private List<GameObject> list = new List<GameObject>();
    private int damage = 10;
    private void Start()
    {
        StartCoroutine(RotateTurret());
    }
    private void Update()
    {
        if (list.Count==0)
            return;
        if (fireTimeLimit <= 0f)
        {
            fireTimeLimit = 1f / fireRate;

            foreach(var i in list)
            {
                Enemy enemy = i.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
            }
        }
        fireTimeLimit -= Time.deltaTime;
    }
    IEnumerator RotateTurret()
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
    private void OnTriggerStay(Collider other)
    {
        list.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        list.Remove(other.gameObject);
    }
}
