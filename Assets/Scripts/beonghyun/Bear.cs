using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    [SerializeField] Transform[] target;
    NavMeshAgent agent;
    Animator anim;
    GameObject nearPlayer;

    List<GameObject> playerDistanceList = new List<GameObject>();

    Vector3 nextPos;

    [SerializeField] float nearDistance;
    //[SerializeField] float moveAmount;
    int targetNumber;

    bool isCoroutine;
    bool isCoroutine2;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        targetNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SetDestination();
        AnimParams();
    }

    void AnimParams()
    {
        if (agent.speed == 0)
        {
            anim.SetBool("WalkForward", false);
            anim.SetBool("Run Forward", false);
        }

        if (agent.speed == 1.5f)
        {
            anim.SetBool("Run Forward", false);
            anim.SetBool("WalkForward", true);
        }

        if (agent.speed == 6f)
        {
            anim.SetBool("Run Forward", true);
            anim.SetBool("WalkForward", false);
            anim.SetBool("Eat", false);
            anim.SetBool("Sit", false);
            anim.SetBool("Sleep", false);
        }
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

    void SetDestination()
    {

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (distance < nearDistance)
        {
            if (isCoroutine2) return;

            if (distance < 4)
            {
                agent.speed = 0;
                string[] states = { "Attack1", "Attack2", "Attack3", "Attack5" };
                int i = Random.Range(0, 4);
                StartCoroutine(AttackState(states[i]));
                return;
            }

            agent.speed = 6f;

            //Vector3 newDir = new Vector3(nearPlayer.transform.position.x-transform.position.x, 0, nearPlayer.transform.position.z - transform.position.z).normalized;
            /*(transform.position - playerPrefab.transform.position).normalized;*/

            agent.destination = nearPlayer.transform.position; //변수화

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

                string[] states = { "Eat","Sit","Sleep"};

                int i = Random.Range(0, 3);

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

    IEnumerator AttackState(string state)
    {
        isCoroutine2 = true;

        transform.LookAt(nearPlayer.transform.position);
        agent.speed = 0;
        anim.SetBool("Combat Idle", true);
        anim.SetTrigger(state);

        yield return new WaitForSeconds(2f);

        anim.SetBool("Combat Idle", false);
        isCoroutine2 = false;
    }

}
