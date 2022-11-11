using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public enum Status
    {
        idle = 1,
        walk,
        attack,
        damaged,
        die
    }
    public static Status status;

    [SerializeField] float hp;
    [SerializeField] float maxHp;
    [SerializeField] int money;

    public bool isCold = false;

    // Callback Methods
    void Start()
    {
        hp = maxHp;
    }

    void Update()
    {
        
    }

    // Public Methods
    // ����
    public void AddHp(float _hp)
    {
        hp += _hp;
    }

    // ����
    public void AddMoney(int _money)
    {
        money += _money;
    }
}
