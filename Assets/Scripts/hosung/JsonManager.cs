using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class SavesDatas
{
    public List<string> playerSavePaths = new List<string>();
    public List<string> roomSavePaths = new List<string>();
    public List<string> turretSavePaths = new List<string>();
    public List<string> houseSavePaths = new List<string>();
}

public class SaveInformations
{
    // Things to Save
    public float curHP;
    public int curMoney;
    public Vector3 playerPosition;
    public List<string> playerItems = new List<string>();
    public List<string> playerEquipments = new List<string>();
}

public class SaveRoomDatas
{
    public bool isPVP;
    public string roomName;
    public int maxPlayer;
    public string password;
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
    public List<bool> turretAuto = new List<bool>();
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
    [SerializeField] private float saveTime = 30f;
    public SaveInformations myInformation;
    public SaveHouseInformations myHouseInformation;
    public SaveTurretInformations myTurretInformation;
    public SaveRoomDatas myRoomInformation;

    public List<GameObject> itemObjects = new List<GameObject>();
    public List<GameObject> turretObjects = new List<GameObject>();

    public SavesDatas savesDatas;
    private string savesPath;

    void Awake()
    {
        instance = this;
        savesPath = Application.streamingAssetsPath + "/saves.json";

        if (File.Exists(savesPath))
        {
            string _fromJsonData = File.ReadAllText(savesPath);
            savesDatas = JsonUtility.FromJson<SavesDatas>(_fromJsonData);
        }
        else savesDatas = new SavesDatas();
    }

    public string SavePlayerData(float _hp, int _money)
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

        string _fileName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_" + PhotonNetwork.LocalPlayer.NickName + "_saveFile";
        string _path = Application.streamingAssetsPath + _fileName + ".json";

        File.WriteAllText(_path, json);

        return _path;
    }

    public void LoadPlayerData()
    {
        string _fileName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_" + PhotonNetwork.LocalPlayer.NickName + "_saveFile";
        string _path = Application.streamingAssetsPath + _fileName + ".json";

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

    public string SaveRoomData()
    {
        string _path = "\n";
        // 방 설정 저장
        SaveRoomDatas saveRoomDatas = new SaveRoomDatas();
        saveRoomDatas.roomName = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"];
        saveRoomDatas.password = (string)PhotonNetwork.CurrentRoom.CustomProperties["password"];
        saveRoomDatas.isPVP = (bool)PhotonNetwork.CurrentRoom.CustomProperties["isPVP"];
        saveRoomDatas.maxPlayer = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxPlayers"];

        string roomJson = JsonUtility.ToJson(saveRoomDatas);
        Debug.Log(roomJson);

        string _roomDatasName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_RoomDataFile";
        string _roomPath = Application.streamingAssetsPath + _roomDatasName + ".json";

        File.WriteAllText(_roomPath, roomJson);

        // 집 저장
        GroundTrigger[] _groundTriggers = GameObject.FindObjectsOfType(typeof(GroundTrigger)) as GroundTrigger[];
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

            string _houseFileName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_HouseSaveFile";
            string _housePath = Application.streamingAssetsPath + _houseFileName + ".json";

            File.WriteAllText(_housePath, houseJson);

            _path += _roomPath + "\n" + _housePath;
        }

        // 터렛 저장 
        SaveTurretInformations saveTurretInformations = new SaveTurretInformations();
        if (GameObject.FindObjectsOfType(typeof(TurretController)) as TurretController[] != null)
        {
            foreach (TurretController turret in GameObject.FindObjectsOfType(typeof(TurretController)) as TurretController[])
            {
                if (turret.trigger == null) //if (turret.IsPublic)
                    continue;
                saveTurretInformations.turretPositions.Add(turret.transform.position);
                saveTurretInformations.turretOwnerNicknames.Add(turret.turretOwner);
                saveTurretInformations.turretAuto.Add(turret.IsAuto);
            }

            string turretJson = JsonUtility.ToJson(saveTurretInformations);
            Debug.Log(turretJson);

            string _turretFileName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_TurretSaveFile";
            string _turretPath = Application.streamingAssetsPath + _turretFileName + ".json";

            File.WriteAllText(_turretPath, turretJson);

            _path += "\n" + _turretPath;
        }

        return _path;
    }

    public void LoadRoomData()
    {
        string _roomDatasName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_RoomDataFile";
        string _roomPath = Application.streamingAssetsPath + _roomDatasName + ".json";
        if (File.Exists(_roomPath))
        {
            string _fromJsonData = File.ReadAllText(_roomPath);
            myRoomInformation = JsonUtility.FromJson<SaveRoomDatas>(_fromJsonData);
            Debug.Log($"pvp : {myRoomInformation.isPVP}, max : {myRoomInformation.maxPlayer}, password : {myRoomInformation.password}");
            NetworkManager.Instance.OnClickCreate(PhotonNetwork.LocalPlayer.NickName, myRoomInformation.maxPlayer, myRoomInformation.password);
        }

        // 집 로드 
        string _houseFileName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_HouseSaveFile";
        string _housePath = Application.streamingAssetsPath + _houseFileName + ".json";

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

        // 터렛 로드
        string _turretFileName = "/" + (string)PhotonNetwork.CurrentRoom.CustomProperties["roomName"] + "_TurretSaveFile";
        string _turretPath = Application.streamingAssetsPath + _turretFileName + ".json";

        if (File.Exists(_turretPath))
        {
            string _fromJsonData = File.ReadAllText(_turretPath);
            myTurretInformation = JsonUtility.FromJson<SaveTurretInformations>(_fromJsonData);
            Debug.Log("Success to load turret data!");

            for (int i = 0; i < myTurretInformation.turretPositions.Count; i++)
            {
                BuildManager _buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
                var newTurret = PhotonNetwork.Instantiate(_buildManager.buildPrefabNameList[(int)BuildType.turret], myTurretInformation.turretPositions[i], Quaternion.identity);
                newTurret.GetComponent<TurretController>().turretOwner = myTurretInformation.turretOwnerNicknames[i];
                newTurret.GetComponent<TurretController>().IsAuto = myTurretInformation.turretAuto[i];
                turretObjects.Add(newTurret.gameObject);

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

    void SaveAllData()
    {
        string saveStrings = SavePlayerData(PlayerStat.LocalPlayer.hp, PlayerStat.LocalPlayer.money);
        if (PhotonNetwork.IsMasterClient) saveStrings += SaveRoomData();
        string[] strings = saveStrings.Split("\n");

        // check player dataList length and delete the oldest data
        if (savesDatas.playerSavePaths != null && !savesDatas.playerSavePaths.Contains(strings[0]) && savesDatas.playerSavePaths.Count >= 4)
        {
            File.Delete(savesDatas.playerSavePaths[0]);
            savesDatas.playerSavePaths.RemoveAt(0);
        }

        // save data
        if (!savesDatas.playerSavePaths.Contains(strings[0])) savesDatas.playerSavePaths.Add(strings[0]);
        else
        {
            int idx = savesDatas.playerSavePaths.IndexOf(strings[0]);
            savesDatas.playerSavePaths[idx] = strings[0];
        }

        // if (isMaster)
        if (strings.Length > 1)
        {
            // check dataLists length and delete the oldest datas
            if (savesDatas.roomSavePaths != null && !savesDatas.roomSavePaths.Contains(strings[1]) && savesDatas.roomSavePaths.Count >= 4)
            {
                File.Delete(savesDatas.roomSavePaths[0]);
                savesDatas.roomSavePaths.RemoveAt(0);
            }
            if (savesDatas.houseSavePaths != null && savesDatas.houseSavePaths.Count >= 4)
            {
                File.Delete(savesDatas.houseSavePaths[0]);
                savesDatas.houseSavePaths.RemoveAt(0);
            }
            if (savesDatas.turretSavePaths != null && savesDatas.turretSavePaths.Count >= 4)
            {
                File.Delete(savesDatas.turretSavePaths[0]);
                savesDatas.turretSavePaths.RemoveAt(0);
            }

            // save data
            if (!savesDatas.roomSavePaths.Contains(strings[1])) savesDatas.roomSavePaths.Add(strings[1]);
            else
            {
                int idx = savesDatas.roomSavePaths.IndexOf(strings[1]);
                savesDatas.roomSavePaths[idx] = strings[1];
            }
            if (!savesDatas.houseSavePaths.Contains(strings[2])) savesDatas.houseSavePaths.Add(strings[2]);
            else
            {
                int idx = savesDatas.houseSavePaths.IndexOf(strings[2]);
                savesDatas.houseSavePaths[idx] = strings[2];
            }
            if (!savesDatas.turretSavePaths.Contains(strings[3])) savesDatas.turretSavePaths.Add(strings[3]);
            else
            {
                int idx = savesDatas.turretSavePaths.IndexOf(strings[3]);
                savesDatas.turretSavePaths[idx] = strings[3];
            }
        }
        string json = JsonUtility.ToJson(savesDatas);
        string _path = savesPath;
        File.WriteAllText(_path, json);
    }

    void Update()
    {
        if (curTime >= saveTime)
        {
            SaveAllData();
            curTime = 0f;
        }
        curTime += Time.deltaTime;
    }
}
