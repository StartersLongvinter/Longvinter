using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Bison : LivingEntity
{
    [SerializeField] Transform[] target;

    Vector3 nextPos;

    [SerializeField] float nearDistance;
    [SerializeField] float moveAmount;
    int targetNumber;

    GameObject nearAnimal;
   
    bool isCoroutine;

    // Start is called before the first frame update


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
        if (agent.speed == 0)
        {
            anim.SetTrigger("Attacked");
        }

        if (agent.speed == 1.5f)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", true);
        }

        if (agent.speed == 6f || agent.speed==10f)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Walk", false);
        }
    }

    void SetDestination()
    {
        if (nearPlayer==null)
        {
            agent.speed = 1.5f;

            //nextPos = target[targetNumber].position;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                SetRandomTarget();

                foreach (var bison in nearAnimals)
                {
                    bison.GetComponent<LivingEntity>().agent.destination = nextPos;
                }

            }

            return;
        }
        // ���ݹ޾����� �߰�

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (isAttacked)
        {
            foreach (var bison in nearAnimals)
            {
                bison.GetComponent<LivingEntity>().agent.speed = 10f;
                bison.GetComponent<LivingEntity>().agent.destination = nearPlayer.transform.position;
            }
        }

        //���ݾȹް� player�� ������ �ٰ����� �� ����

        else if (distance < nearDistance && !isAttacked)
        {

            //����� �ִ� ������ �ٽ� ���ư��� �� ����
            SetRandomTarget();

            agent.speed = 6f;

            Vector3 newDir = new Vector3(transform.position.x - nearPlayer.transform.position.x, 0, transform.position.z - nearPlayer.transform.position.z).normalized;

            agent.destination = transform.position + newDir * moveAmount; //����ȭ
        }

       

        //��� ���� ������ġ�� nextPos �Ÿ��� ~~���� �۴ٸ� nextPos �ٲ��� 
        else
        {
            agent.speed = 1.5f;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                //int newTargetNumber = Random.Range(0, target.Length);
                SetRandomTarget();

                foreach (var bison in nearAnimals)
                {
                    bison.GetComponent<LivingEntity>().agent.destination = nextPos;
                }
            }
        }
    }
    void SetRandomTarget()
    {
        nextPos = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)).normalized * moveAmount;
    }
}
