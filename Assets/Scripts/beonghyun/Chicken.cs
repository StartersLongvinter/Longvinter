using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Chicken : LivingEntity
{
    [SerializeField] Transform[] target;
    
    Vector3 nextPos;

    [SerializeField] float nearDistance;
    [SerializeField] float moveAmount;
    int targetNumber;

    bool isCoroutine;


    // Start is called before the first frame update
    void Start()
    {

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
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }

        if (agent.speed == 1.5f)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);
        }

        if (agent.speed == 4.5f)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Walk", false);
            anim.SetBool("Eat", false);
            anim.SetBool("Turn Head", false);
        }
    }

    void SetDestination()
    {
        if (isStopped())
        {
            SetRandomTarget();
        }

        if (nearPlayer==null)
        {
            if (isCoroutine) return;

            agent.speed = 1.5f;

            //nextPos = target[targetNumber].position;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                //int newTargetNumber = Random.Range(0, target.Length);

                //targetNumber = newTargetNumber;

                SetRandomTarget();

                string[] states = { "Eat", "Turn Head" };

                int i = Random.Range(0, 2);

                StartCoroutine(IdleState(states[i]));
            }

            return;
        }
        // 도주할때 조건

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (distance < nearDistance)
        {
            //사람이 있던 곳으로 다시 돌아가는 것 방지
            //targetNumber = Random.Range(0, target.Length);
            SetRandomTarget();

            agent.speed = 4.5f;

            Vector3 newDir = new Vector3(transform.position.x - nearPlayer.transform.position.x, 0, transform.position.z - nearPlayer.transform.position.z).normalized;

            agent.destination = transform.position + newDir * moveAmount; //변수화
        }

        //평소 조건 현재위치와 nextPos 거리가 ~~보다 작다면 nextPos 바꿔줌 
        else
        {
            if (isCoroutine) return;

            agent.speed = 1.5f;

            //nextPos = target[targetNumber].position;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                //int newTargetNumber = Random.Range(0, target.Length);

                //targetNumber = newTargetNumber;

                SetRandomTarget();

                string[] states = { "Eat", "Turn Head" };

                int i = Random.Range(0, 2);

                StartCoroutine(IdleState(states[i]));
            }
        }
    }

    void SetRandomTarget()
    {
        nextPos = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)).normalized * moveAmount;
    }

    IEnumerator IdleState(string state)
    {
        isCoroutine = true;

        agent.speed = 0;

        anim.SetBool(state, true);

        yield return new WaitForSeconds(4f);

        anim.SetBool(state, false);

        isCoroutine = false;
    }
}
