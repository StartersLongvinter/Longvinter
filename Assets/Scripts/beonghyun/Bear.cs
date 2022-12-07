using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Bear : LivingEntity
{
    Vector3 nextPos;

    [SerializeField] float nearDistance;
    [SerializeField] float moveAmount;

    bool isCoroutine;
    bool isCoroutine2;

    [SerializeField] GameObject attackArea;

    public float damage = 15; 

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

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
    
    void SetDestination()
    {
      

        if (nearPlayer == null)
        {
            if (isAttacked)
            {
                agent.speed = 6f;
                agent.destination = transform.position + bulletDir.normalized * (-moveAmount);
                return;
            }

            if (isCoroutine) return;

            agent.speed = 1.5f;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                SetRandomTarget();

                string[] states = { "Eat", "Sit", "Sleep" };

                int i = Random.Range(0, 3);

                StartCoroutine(IdleState(states[i]));
            }

            return;
        }

        else
        {
                float _distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

                if (_distance < nearDistance || isAttacked)
                {

                    SetRandomTarget();

                    if (isCoroutine2) return;

                    if (_distance < 4)
                    {
                        agent.speed = 0;
                        string[] states = { "Attack1", "Attack2", "Attack3", "Attack5" };
                        int i = Random.Range(0, 4);
                        StartCoroutine(AttackState(states[i]));
                        return;
                    }

                    agent.speed = 6f;

                    agent.destination = nearPlayer.transform.position;

            }
            
            //평소 조건 현재위치와 nextPos 거리가 ~~보다 작다면 nextPos 바꿔줌 
            else
            {
                if (isCoroutine) return;

                agent.speed = 1.5f;

                float distance = Vector3.Distance(transform.position, nextPos);

                agent.destination = nextPos;

                if (distance < 1)
                {
                    SetRandomTarget();

                    string[] states = { "Eat", "Sit", "Sleep" };

                    int i = Random.Range(0, 3);

                    StartCoroutine(IdleState(states[i]));
                }
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

    IEnumerator AttackState(string state)
    {
        isCoroutine2 = true;

        transform.LookAt(nearPlayer.transform.position);
        agent.speed = 0;
        anim.SetBool("Combat Idle", true);
        anim.SetTrigger(state);

        yield return new WaitForSeconds(0.3f);

        attackArea.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        anim.SetBool("Combat Idle", false);
        isCoroutine2 = false;
        attackArea.SetActive(false);
    }

    //protected override void OnTriggerEnter(Collider other)
    //{
    //    base.OnTriggerEnter(other);
    //    if (other.gameObject.tag=="Player")
    //    {
    //        Debug.Log("attack player");
    //    }
    //}

}
