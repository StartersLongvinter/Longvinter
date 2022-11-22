using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LivingEntity : MonoBehaviour
{
    //[SerializeField] Transform[] target;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    //[SerializeField] float nearDistance;
    //[SerializeField] float moveAmount;

    //float damageAmount=1;
    //int targetNumber;
    public NavMeshAgent agent;
    public Animator anim;
    SkinnedMeshRenderer meshes;
    public GameObject nearPlayer;
    List<GameObject> playerDistanceList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //targetNumber = 0;
        maxHealth = 100;
        currentHealth = 100;
        meshes = GetComponentInChildren<SkinnedMeshRenderer>();
        InvokeRepeating("HitByPlayer", 2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        //HitByPlayer();
    }

    //제일 가까이 있는 player를 nearPlayer로 지정

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerDistanceList.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        float minDistance = float.MaxValue;

        foreach (var player in playerDistanceList)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (minDistance > distance)
            {
                minDistance = distance;
                nearPlayer = player;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerDistanceList.Remove(other.gameObject);
            //myDict.Remove(other.gameObject);
        }
    }

    public void HitByPlayer()
    {
        StartCoroutine(OnDamage());
    }



    IEnumerator OnDamage()
    {

        meshes.material.color = Color.red;
        transform.localScale += new Vector3(0.3f,0.3f,0.3f);
        yield return new WaitForSeconds(0.1f);

        meshes.material.color = Color.blue;
        transform.localScale -= new Vector3(0.3f,0.3f,0.3f);
        yield return new WaitForSeconds(0.1f);


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


}
