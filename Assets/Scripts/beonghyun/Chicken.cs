using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    [SerializeField]float maxHealth;
    [SerializeField]float currentHealth;
    //float damageAmount=1;

    SkinnedMeshRenderer meshes;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
        meshes = GetComponentInChildren<SkinnedMeshRenderer>();
        //InvokeRepeating("HitByPlayer", 2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        //HitByPlayer();
    }

    
    public void HitByPlayer()
    {
        StartCoroutine(OnDamage());
    }

    

    IEnumerator OnDamage()
    {
      
        meshes.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        meshes.material.color = Color.blue;

        yield return new WaitForSeconds(0.1f);

        currentHealth -= 10;

        if (currentHealth > 0)
        {
            meshes.material.color = Color.white;
        }

        else
        {
            //foreach (MeshRenderer mesh in meshes)
            //{
            //    mesh.material.color = Color.gray;
            //}

            Destroy(gameObject);

        }


    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
