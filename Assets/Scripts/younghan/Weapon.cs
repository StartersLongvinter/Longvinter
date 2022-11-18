using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public enum AttackMode { Single, Auto }

    public Type type;
    public AttackMode attackMode;
    public float damage;
    public float rate;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
