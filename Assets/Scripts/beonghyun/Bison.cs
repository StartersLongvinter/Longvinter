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
    //public List<GameObject> nearAnimals = new List<GameObject>();
   
    bool isCoroutine;

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        SetDestination();
        AnimParams();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitByPlayer();
        }
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

            nextPos = target[targetNumber].position;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                int newTargetNumber = Random.Range(0, target.Length);

                foreach (var bison in nearAnimals)
                {
                    //int newTargetNumber = Random.Range(0, target.Length);

                    bison.GetComponent<Bison>().targetNumber = newTargetNumber;
                }
                //int newTargetNumber = Random.Range(0, target.Length);

                //targetNumber = newTargetNumber;

            }

            return;
        }
        // 공격받았을때 추격

        float distance = Vector3.Distance(nearPlayer.transform.position, transform.position);

        if (isAttacked)
        {
            //if (isCoroutine) return;


            //StartCoroutine(AttackedState());
            foreach (var bison in nearAnimals)
            {
                bison.GetComponent<LivingEntity>().agent.speed = 10f;
                bison.GetComponent<LivingEntity>().agent.destination = nearPlayer.transform.position;
            }
            //agent.speed = 10f;
            //agent.destination = nearPlayer.transform.position;



        }

        //공격안받고 player가 가까이 다가갔을 때 도주

        else if (distance < nearDistance && !isAttacked)
        {

            //사람이 있던 곳으로 다시 돌아가는 것 방지
            targetNumber = Random.Range(0, target.Length);

            agent.speed = 6f;

            Vector3 newDir = new Vector3(transform.position.x - nearPlayer.transform.position.x, 0, transform.position.z - nearPlayer.transform.position.z).normalized;
            /*(transform.position - playerPrefab.transform.position).normalized;*/

            agent.destination = transform.position + newDir * moveAmount; //변수화
        }

        // 공격당하면 멈춰서 도발 coroutine 한번 갈겨주고 nearPlayer 추격
       

        //평소 조건 현재위치와 nextPos 거리가 ~~보다 작다면 nextPos 바꿔줌 
        else
        {

            agent.speed = 1.5f;

            nextPos = target[targetNumber].position;

            float _distance = Vector3.Distance(transform.position, nextPos);

            agent.destination = nextPos;

            if (_distance < 1)
            {
                int newTargetNumber = Random.Range(0, target.Length);

                foreach (var bison in nearAnimals)
                {
                    //int newTargetNumber = Random.Range(0, target.Length);

                    bison.GetComponent<Bison>().targetNumber = newTargetNumber;
                }
                //int newTargetNumber = Random.Range(0, target.Length);

                //targetNumber = newTargetNumber;

            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag=="Bison")
    //    {
    //        nearAnimals.Add(other.gameObject);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Bison")
    //    {
    //        nearAnimals.Remove(other.gameObject);
    //    }
    //}

    //IEnumerator AttackedState()
    //{
    //    isCoroutine = true;

    //    transform.LookAt(nearPlayer.transform);

    //    agent.speed = 0;

    //    yield return new WaitForSeconds(1.5f);

    //    agent.speed = 10;
    //    agent.destination = nearPlayer.transform.position;

    //    yield return new WaitForSeconds(4f);

    //    isCoroutine = false;
    //    //StopAllCoroutines();
    //}

}
