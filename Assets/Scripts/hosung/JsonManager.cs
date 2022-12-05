using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SaveInformations
{
    // Things to Save
    public float curHP;
    public int curMoney;
    public Vector3 playerPosition;
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

    void Awake()
    {

    }

    public void SaveData(float _hp, int _money)
    {
        SaveInformations saveInformations = new SaveInformations();
        saveInformations.curHP = _hp;
        saveInformations.curMoney = _money;
        saveInformations.playerPosition = PlayerStat.LocalPlayer.gameObject.transform.position;

        string json = JsonUtility.ToJson(saveInformations);
        Debug.Log(json);

        string _fileName = PhotonNetwork.CurrentRoom.Name + "_" + PhotonNetwork.LocalPlayer.NickName + "_saveFile";
        string _path = Application.dataPath + "/SaveFiles/" + _fileName + ".json";

        File.WriteAllText(_path, json);
    }

    public void LoadDate()
    {
        string _fileName = PhotonNetwork.CurrentRoom.Name + "_" + PhotonNetwork.LocalPlayer.NickName + "_saveFile";
        string _path = Application.dataPath + "/SaveFiles/" + _fileName + ".json";

        if (File.Exists(_path))
        {
            string _fromJsonData = File.ReadAllText(_path);
            myInformation = JsonUtility.FromJson<SaveInformations>(_fromJsonData);
            Debug.Log("Success to load data!");

            PlayerStat.LocalPlayer.transform.position = myInformation.playerPosition;
            PlayerStat.LocalPlayer.hp = myInformation.curHP;
            PlayerStat.LocalPlayer.money = myInformation.curMoney;
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
