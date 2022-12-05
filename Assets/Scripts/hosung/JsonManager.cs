using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

public class SaveInformations
{
    // Things to Save
    public float curHP;
    public int curMoney;
    public Vector3 playerPosition;
    public List<string> playerItems = new List<string>();
    public List<string> playerEquipments = new List<string>();
}

public class JsonManager : MonoBehaviour
{
    private static JsonManager instance;
    public static JsonManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj;
                obj = GameObject.Find("JsonManager");
                if (obj == null)
                {
                    obj = new GameObject("JsonManager");
                    // obj.GetComponent<PhotonView>().TransferOwnership()
                    instance = obj.AddComponent<JsonManager>();
                }
                else
                {
                    obj.AddComponent<JsonManager>();
                    instance = obj.GetComponent<JsonManager>();
                }
            }
            return instance;
        }
    }

    private float curTime = 0f;
    private float saveTime = 30f;
    public SaveInformations myInformation;

    public List<GameObject> itemObjects = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    public void SaveData(float _hp, int _money)
    {
        SaveInformations saveInformations = new SaveInformations();
        saveInformations.curHP = _hp;
        saveInformations.curMoney = _money;
        saveInformations.playerPosition = PlayerStat.LocalPlayer.gameObject.transform.position;
        // saveInformations.playerItems = PlayerStat.LocalPlayer.GetComponent<PlayerInventory>().itemList;

        foreach (GameObject item in PlayerStat.LocalPlayer.GetComponent<PlayerInventory>().itemList)
        {
            saveInformations.playerItems.Add(item.name.Replace("(Clone)", ""));
        }
        foreach (GameObject equipment in PlayerStat.LocalPlayer.GetComponent<PlayerInventory>().equipmentList)
        {
            saveInformations.playerEquipments.Add(equipment.name.Replace("(Clone)", ""));
        }

        string json = JsonUtility.ToJson(saveInformations);
        Debug.Log(json);

        string _fileName = PhotonNetwork.CurrentRoom.Name + "_" + PhotonNetwork.LocalPlayer.NickName + "_saveFile";
        string _path = Application.dataPath + _fileName + ".json";

        File.WriteAllText(_path, json);
    }

    public void LoadDate()
    {
        string _fileName = PhotonNetwork.CurrentRoom.Name + "_" + PhotonNetwork.LocalPlayer.NickName + "_saveFile";
        string _path = Application.dataPath + _fileName + ".json";

        if (File.Exists(_path))
        {
            string _fromJsonData = File.ReadAllText(_path);
            myInformation = JsonUtility.FromJson<SaveInformations>(_fromJsonData);
            Debug.Log("Success to load data!");

            PlayerStat.LocalPlayer.transform.position = myInformation.playerPosition;
            PlayerStat.LocalPlayer.hp = myInformation.curHP;
            PlayerStat.LocalPlayer.money = myInformation.curMoney;

            foreach (string itemName in myInformation.playerItems)
            {
                foreach (GameObject _item in itemObjects)
                {
                    if (_item.name == itemName)
                    {
                        PlayerStat.LocalPlayer.GetComponent<PlayerInventory>().AddItem(_item, false);
                        break;
                    }
                }
            }
            foreach (string _equipmentName in myInformation.playerEquipments)
            {
                foreach (GameObject _item in itemObjects)
                {
                    if (_item.name == _equipmentName)
                    {
                        PlayerStat.LocalPlayer.GetComponent<PlayerInventory>().AddItem(_item, false);
                        break;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (curTime >= saveTime)
        {
            SaveData(PlayerStat.LocalPlayer.hp, PlayerStat.LocalPlayer.money);
            curTime = 0f;
        }
        curTime += Time.deltaTime;
    }
}
