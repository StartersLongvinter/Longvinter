using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public enum Status
    {
        Idle = 1,
        Walk,
        Attack,
        Damaged,
        Die
    }
    public static Status status;

    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] int money;

    public bool isCold = false;

    void Start()
    {
        hp = maxHp;
    }

    void Update()
    {
        
    }

    public void AddHp(float _hp)
    {
        hp += _hp;
    }

    public void AddMoney(int _money)
    {
        money += _money;
    }
}
