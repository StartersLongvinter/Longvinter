using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    public string playerPrefabName = "";
    [SerializeField] GameObject roomPrefab;

    void Awake()
    {
        NetworkManager.Instance.Init(playerPrefabName, roomPrefab);
    }

    void Update()
    {

    }
}
