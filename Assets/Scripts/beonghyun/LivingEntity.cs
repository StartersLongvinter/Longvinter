using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    
    [SerializeField] Material hitEffect1;
    [SerializeField] Material hitEffect2;

    //[SerializeField] GameObject defaultItem;
   
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

    //Bullet_BH ����
    Bullet_BH bullet;
    public Vector3 bulletDir;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //maxHealth = 100;
        //currentHealth = 100;
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

    //public bool isStopped()
    //{
    //    float timer = 0;

    //    while (currentPosition==latePosition)
    //    {
    //        timer += Time.deltaTime;

    //        if (timer>5)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //���� ������ �ִ� player�� nearPlayer�� ����

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Bullet")
        {
            bullet = collision.gameObject.GetComponent<Bullet_BH>();
            bulletDir = transform.position - bullet.transform.position;
        }
    }

    public void HitByPlayer(float damage)
    {
        StartCoroutine(OnDamageEffect());
        OnDamage(damage);
    }

    void OnDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth<0 && photonView.IsMine)
        {
            PhotonNetwork.Instantiate("DefaultItem", this.gameObject.transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(this.gameObject);
           
        }
    }

    IEnumerator OnDamageEffect()
    {
        isAttacked = true;

        mesh.material = hitEffect1; 
        transform.localScale += new Vector3(0.01f,0.01f,0.01f);

        yield return new WaitForSeconds(0.1f);

        mesh.material = hitEffect2;
        transform.localScale -= new Vector3(0.01f,0.01f,0.01f);

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            mesh.material = startMat;
        }

        yield return new WaitForSeconds(4f);
        isAttacked = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }

        else
        {
            currentHealth = (float)stream.ReceiveNext();
        }
    }
}
