using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Chicken : LivingEntity
{
    [SerializeField] Transform[] target;
    
    Vector3 nextPos;

    [SerializeField] float nearDistance;
    [SerializeField] float moveAmount;
    int targetNumber;

    bool isCoroutine;

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            SetDestination();
            AnimParams();
        }
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
        if (nearPlayer==null)
        {
            if (isAttacked)
            {
                agent.speed = 4.5f;
                agent.destination = transform.position + bulletDir.normalized * moveAmount;
                return;
            }

            if (isCoroutine) return;

            agent.speed = 1.5f;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                SetRandomTarget();

                string[] states = { "Eat", "Turn Head" };

                int i = Random.Range(0, 2);

                StartCoroutine(IdleState(states[i]));
            }

            return;
        }
        // 도주할때 조건

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (distance < nearDistance || isAttacked)
        {
            //사람이 있던 곳으로 다시 돌아가는 것 방지
           
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

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
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
