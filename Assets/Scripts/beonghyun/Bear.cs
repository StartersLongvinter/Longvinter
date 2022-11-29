using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Bear : LivingEntity
{
    [SerializeField] Transform[] target;

    Vector3 nextPos;

    [SerializeField] float nearDistance;
    [SerializeField] float moveAmount;
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

                string[] states = { "Eat", "Sit", "Sleep" };

                int i = Random.Range(0, 3);

                StartCoroutine(IdleState(states[i]));
            }

            return;
        }

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (distance < nearDistance)
        {

            SetRandomTarget();

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

            agent.destination = nearPlayer.transform.position; 

        }

        //��� ���� ������ġ�� nextPos �Ÿ��� ~~���� �۴ٸ� nextPos �ٲ��� 
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

                string[] states = { "Eat","Sit","Sleep"};

                int i = Random.Range(0, 3);

                StartCoroutine(IdleState(states[i]));
            }
        }

        if (true)
        {

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
