using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerList : MonoBehaviour
{
    private static PlayerList instance;
    public static PlayerList Instance { get { return instance; } }

    public List<Player> players = new List<Player>();
    public List<PlayerStat> playerStats = new List<PlayerStat>();

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }
}
