using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbineBlade : MonoBehaviour
{
    private float x;
    // Start is called before the first frame update
    void Start()
    {
        x = 0;
    }

    // Update is called once per frame
    void Update()
    {
        x = Time.deltaTime * 200f;
        
        transform.Rotate(x , 0, 0);
    }
}
