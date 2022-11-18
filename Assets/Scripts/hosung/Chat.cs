using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{
    Animator chatAnimator;
    public float rotationValue = 1f;

    void Awake()
    {
        chatAnimator = GetComponent<Animator>();
    }

    public void TurnOffChat()
    {
        StartCoroutine(FadeOutChat());
    }

    IEnumerator FadeOutChat()
    {
        yield return new WaitForSeconds(3f);
        chatAnimator.SetTrigger("TurnOff");
    }

    public void DestroyChat()
    {
        Destroy(transform.parent.gameObject);
    }

    void Update()
    {

    }
}
