using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningText : MonoBehaviour
{
    float curTime = 3f;
    public bool isStart = false;
    Text warningText;

    void Awake()
    {
        warningText = GetComponent<Text>();
    }

    void Update()
    {
        if (isStart)
        {
            if (curTime <= 0f)
            {
                curTime = 3f;
                isStart = false;
                warningText.text = "";
            }

            if (curTime > 0)
            {
                curTime -= Time.deltaTime;
            }
        }


    }
}
