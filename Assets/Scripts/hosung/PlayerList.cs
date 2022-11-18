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
    public List<GameObject> playerCharacters = new List<GameObject>();
    public Dictionary<int, GameObject> playersWithActorNumber = new Dictionary<int, GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }
}
