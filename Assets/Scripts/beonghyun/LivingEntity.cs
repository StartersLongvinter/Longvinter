using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LivingEntity : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    
    [SerializeField] Material hitEffect1;
    [SerializeField] Material hitEffect2;
   
    Material startMat;
    public NavMeshAgent agent;
    public Animator anim;
    SkinnedMeshRenderer mesh;
    public GameObject nearPlayer;
    
    List<GameObject> playerDistanceList = new List<GameObject>();
    public List<GameObject> nearAnimals = new List<GameObject>();
    public bool isAttacked;

    Vector3 currentPosition;
    Vector3 latePosition;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        maxHealth = 100;
        currentHealth = 100;
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        startMat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        
        nearAnimals.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.position;
    }

    //private void LateUpdate()
    //{
    //    latePosition = transform.position;
    //}

    public bool isStopped()
    {
        float timer = 0;

        while (currentPosition==latePosition)
        {
            timer += Time.deltaTime;

            if (timer>5)
            {
                return true;
            }
        }

        return false;
    }

    //제일 가까이 있는 player를 nearPlayer로 지정

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerDistanceList.Add(other.gameObject);
        }

        if (other.gameObject.tag == "Bison" && !nearAnimals.Contains(other.gameObject) && this.gameObject.tag=="Bison")
        {
            Debug.Log("Bison");
            nearAnimals.Add(other.gameObject);
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
        }

        if (other.gameObject.tag == "Bison")
        {
            nearAnimals.Remove(other.gameObject);
        }
    }

    public void HitByPlayer()
    {
        StartCoroutine(OnDamage());
    }

    IEnumerator OnDamage()
    {
        isAttacked = true;
        mesh.material = hitEffect1; 
        transform.localScale += new Vector3(0.1f,0.1f,0.1f);

        yield return new WaitForSeconds(0.1f);

        mesh.material = hitEffect2;
        transform.localScale -= new Vector3(0.1f,0.1f,0.1f);

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            mesh.material = startMat;
        }
        else
        {
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(4f);
        isAttacked = false;
    }
}
