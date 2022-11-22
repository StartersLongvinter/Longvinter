using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : LivingEntity
{
    [SerializeField] Transform[] target;
    //NavMeshAgent agent;
    //Animator anim;
    //GameObject nearPlayer;
    
    //List<GameObject> playerDistanceList = new List<GameObject>();

    Vector3 nextPos;
    
    [SerializeField] float nearDistance;
    [SerializeField] float moveAmount;
    int targetNumber;

    bool isCoroutine;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        //anim = GetComponent<Animator>();
        
        //targetNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SetDestination();
        AnimParams();

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    HitByPlayer();
        //}
    }

    void AnimParams()
    {
        if (agent.speed==0)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }

        if (agent.speed==1.5f)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);
        }

        if (agent.speed==4.5f)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Walk", false);
            anim.SetBool("Eat", false);
            anim.SetBool("Turn Head", false);
        }
    }

    //제일 가까이 있는 player를 nearPlayer로 지정

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        playerDistanceList.Add(other.gameObject);
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    float minDistance = float.MaxValue;

    //    foreach (var player in playerDistanceList)
    //    {
    //        float distance = Vector3.Distance(player.transform.position, transform.position);

    //        if (minDistance > distance)
    //        {
    //            minDistance = distance;
    //            nearPlayer = player;
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        playerDistanceList.Remove(other.gameObject);
    //        //myDict.Remove(other.gameObject);
    //    }
    //}

    void SetDestination()
    {
        // 도주할때 조건

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (distance < nearDistance)
        {
            //사람이 있던 곳으로 다시 돌아가는 것 방지
            targetNumber = Random.Range(0, target.Length);

            agent.speed = 4.5f;

            Vector3 newDir = new Vector3(transform.position.x - nearPlayer.transform.position.x, 0, transform.position.z - nearPlayer.transform.position.z).normalized;
            /*(transform.position - playerPrefab.transform.position).normalized;*/

            agent.destination = transform.position + newDir * moveAmount; //변수화
        }

        //평소 조건 현재위치와 nextPos 거리가 ~~보다 작다면 nextPos 바꿔줌 
        else
        {
            if (isCoroutine) return;

            agent.speed = 1.5f;

            nextPos = target[targetNumber].position;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                int newTargetNumber = Random.Range(0, target.Length);

                targetNumber = newTargetNumber;

                string[] states = { "Eat", "Turn Head" };

                int i = Random.Range(0, 2);

                StartCoroutine(IdleState(states[i]));
            }
        }
    }

    IEnumerator IdleState(string state)
    {
        isCoroutine = true;

        agent.speed = 0;

        anim.SetBool(state, true);

        yield return new WaitForSeconds(4f);

        anim.SetBool(state, false);

        isCoroutine = false;
        //StopAllCoroutines();
    }
    


}
