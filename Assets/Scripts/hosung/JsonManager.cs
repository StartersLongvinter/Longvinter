using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class SaveInformations
{
    // Things to Save
    public float curHP;
    public int curMoney;
    public Vector3 playerPosition;
    public List<string> playerItems = new List<string>();
    public List<string> playerEquipments = new List<string>();
}

public class SaveHouseInformations
{
    // Have to add nickname at his house index!!
    public List<Vector3> housePositions = new List<Vector3>();
    public List<string> houseOwnerNicknames = new List<string>();
}

public class SaveTurretInformations
{
    // Have to add nickname at his turret index!!
    public List<Vector3> turretPositions = new List<Vector3>();
    public List<string> turretOwnerNicknames = new List<string>();
}

public class JsonManager : MonoBehaviourPun
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
    public SaveHouseInformations myHouseInformation;
    public SaveTurretInformations myTurretInformation;

    public List<GameObject> itemObjects = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    public void SavePlayerData(float _hp, int _money)
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

    public void LoadPlayerDate()
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

    public void SaveRoomData()
    {
        GroundTrigger[] _groundTriggers = GameObject.FindObjectsOfType(typeof(GroundTrigger)) as GroundTrigger[];
        // 집 저장
        SaveHouseInformations saveHouseInformations = new SaveHouseInformations();
        if (GameObject.FindObjectsOfType(typeof(GroundTrigger)) as GroundTrigger[] != null)
        {
            foreach (GroundTrigger homeArea in _groundTriggers)
            {
                saveHouseInformations.housePositions.Add(homeArea.transform.position);
                saveHouseInformations.houseOwnerNicknames.Add(homeArea.gameObject.name.Replace("HomeArea", ""));
            }

            string houseJson = JsonUtility.ToJson(saveHouseInformations);
            Debug.Log(houseJson);

            string _houseFileName = PhotonNetwork.CurrentRoom.Name + "_HouseSaveFile";
            string _housePath = Application.dataPath + _houseFileName + ".json";

            File.WriteAllText(_housePath, houseJson);
        }

        // 터렛 저장 
        SaveTurretInformations saveTurretInformations = new SaveTurretInformations();
        if (GameObject.FindObjectsOfType(typeof(TurretController)) as TurretController[] != null)
        {
            foreach (TurretController turret in GameObject.FindObjectsOfType(typeof(TurretController)) as TurretController[])
            {
                saveTurretInformations.turretPositions.Add(turret.transform.position);
                saveTurretInformations.turretOwnerNicknames.Add(turret.turretOwner);
            }

            string turretJson = JsonUtility.ToJson(saveTurretInformations);
            Debug.Log(turretJson);

            string _turretFileName = PhotonNetwork.CurrentRoom.Name + "_TurretSaveFile";
            string _turretPath = Application.dataPath + _turretFileName + ".json";

            File.WriteAllText(_turretPath, turretJson);
        }
    }

    public void LoadRoomData()
    {
        string _houseFileName = PhotonNetwork.CurrentRoom.Name + "_HouseSaveFile";
        string _housePath = Application.dataPath + _houseFileName + ".json";

        GroundTrigger[] _groundTriggers = GameObject.FindObjectsOfType(typeof(GroundTrigger)) as GroundTrigger[];

        if (File.Exists(_housePath))
        {
            string _fromJsonData = File.ReadAllText(_housePath);
            myHouseInformation = JsonUtility.FromJson<SaveHouseInformations>(_fromJsonData);
            Debug.Log("Success to load house data!");

            for (int i = 0; i < myHouseInformation.housePositions.Count; i++)
            {
                BuildManager _buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
                var newHouse = PhotonNetwork.Instantiate(BuildType.house.ToString(), myHouseInformation.housePositions[i], Quaternion.identity);

                var newHomeArea = PhotonNetwork.Instantiate(_buildManager.homeAreaPrefabName, myHouseInformation.housePositions[i], Quaternion.identity);
                newHomeArea.gameObject.name = myHouseInformation.houseOwnerNicknames[i] + "HomeArea";
            }
        }

        string _turretFileName = PhotonNetwork.CurrentRoom.Name + "_TurretSaveFile";
        string _turretPath = Application.dataPath + _turretFileName + ".json";

        if (File.Exists(_turretPath))
        {
            string _fromJsonData = File.ReadAllText(_turretPath);
            myTurretInformation = JsonUtility.FromJson<SaveTurretInformations>(_fromJsonData);
            Debug.Log("Success to load turret data!");

            for (int i = 0; i < myTurretInformation.turretPositions.Count; i++)
            {
                BuildManager _buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
                var newTurret = PhotonNetwork.Instantiate(_buildManager.buildPrefabNameList[(int)BuildType.turret], myTurretInformation.turretPositions[i], Quaternion.identity);
                // newHouse.GetComponent<TurretController>().turretOwner = myTurretInformation.turretOwnerNicknames[i];
                newTurret.GetComponent<TurretController>().turretOwner = myTurretInformation.turretOwnerNicknames[i];

                float distance = 10000;
                GroundTrigger _groundTrigger = new GroundTrigger();

                foreach (GroundTrigger _triggerGround in _groundTriggers)
                {
                    if (Vector3.Distance(myTurretInformation.turretPositions[i], _triggerGround.transform.position) < distance)
                    {
                        _groundTrigger = _triggerGround;
                    }
                }
                newTurret.GetComponent<TurretController>().trigger = _groundTrigger;
            }
        }
    }

    void Update()
    {
        if (curTime >= saveTime)
        {
            SavePlayerData(PlayerStat.LocalPlayer.hp, PlayerStat.LocalPlayer.money);
            if (PhotonNetwork.IsMasterClient) SaveRoomData();
            curTime = 0f;
        }
        curTime += Time.deltaTime;
    }
}
