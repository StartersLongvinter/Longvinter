using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public enum BuildType
{
    none = 0,
    turret,
    house,
    other,
    countIndex // don��t set this type! This type only use to check enum��s length.
}
public class BuildManager : MonoBehaviourPun
{
    BuildType buildType = BuildType.none;
    public Vector3 mousePosition;
    [SerializeField] GameObject[] buildObjectPrefab;
    GameObject buildObject;
    public bool canBuild = true;
    GameObject[] buildArea;
    [SerializeField] Color[] buildObjectColors;
    List<GameObject> myHomeAreas = new List<GameObject>();
    GameObject myHomeArea;
    string buildPrefabName = "";
    [SerializeField] string[] buildPrefabNameList;
    List<GameObject> homeAreas = new List<GameObject>();
    [SerializeField] string homeAreaPrefabName;

    void Awake()
    {

    }

    public void SetBuildType(int buildtypeNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PlayerStat.LocalPlayer.ownerPlayerActorNumber)
            return;

        buildType = BuildType.none;
        Destroy(buildObject);
        buildObject = null;

        buildType = (BuildType)buildtypeNumber;
        buildPrefabName = buildPrefabNameList[(int)buildType];
        buildObject = Instantiate(buildObjectPrefab[(buildtypeNumber)], Vector3.zero, Quaternion.identity);

        PlayerStat.LocalPlayer.gameObject.GetComponent<PlayerController>().isBuilding = true;
    }
    bool CheckBuildPosition(Vector3 _mousePosition)
    {
        if (myHomeArea == null)
            foreach (GameObject area in buildArea) if (area.name == PhotonNetwork.LocalPlayer.NickName + "HomeArea") myHomeAreas.Add(area);
        if (buildType == BuildType.turret)
        {
            if (myHomeAreas.Count <= 0 || Vector3.Distance(PlayerStat.LocalPlayer.gameObject.transform.position, _mousePosition) > 4f)
            {
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                return false;
            }
            float _d = 1000f;
            foreach (GameObject _homeArea in myHomeAreas)
            {
                float _thisHomeDistance = Vector3.Distance(_mousePosition, _homeArea.transform.position);
                if (_d >= _thisHomeDistance)
                {
                    _d = _thisHomeDistance;
                    myHomeArea = _homeArea;
                }
            }
            // float _distance = Vector3.Distance(myHomeArea.transform.position, _mousePosition);
            Debug.Log(_d);
            if (_d > (myHomeArea.transform.lossyScale.x * 0.5f) - buildObject.transform.localScale.x * 0.5f)
            {
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                return false;
            }
        }
        else if (buildType == BuildType.house)
        {
            if (Vector3.Distance(PlayerStat.LocalPlayer.gameObject.transform.position, _mousePosition) > 4f)
            {
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                return false;
            }
            float _distance = 1000f;
            float homeAreaRadius = -1f;
            foreach (GameObject home in buildArea)
            {
                if (home.TryGetComponent(out GroundTrigger groundTrigger) == false)
                    continue;
                if (homeAreaRadius == -1f) homeAreaRadius = home.transform.lossyScale.x;
                float _d = Vector3.Distance(home.transform.position, _mousePosition);
                if (_distance > _d)
                    _distance = _d;
            }
            Debug.Log(_distance);
            if (_distance <= homeAreaRadius)
            {
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                return false;
            }
        }
        buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[0];
        return true;
    }
    void Update()
    {
        GetInput();
        if (buildType == BuildType.none) return;
        buildArea = GameObject.FindGameObjectsWithTag("Area");
        foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = true;
        // mousePosition = GameObject.Find(��UICamera��).GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        RaycastHit hit;
        Vector3 mouseVector = Input.mousePosition;
        mouseVector.z = Camera.main.farClipPlane;
        Vector3 dir = Camera.main.ScreenToWorldPoint(mouseVector);
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);
        Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red);
        // int layerMask = 1 << LayerMask.NameToLayer(��Player��) | 1 << LayerMask.NameToLayer(��Area��);
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("Area"));
        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                canBuild = false;
            }
            else
            {
                canBuild = CheckBuildPosition(hit.point);
            }
            // if (hit.transform.gameObject != buildObject)
            mousePosition = hit.point;
        }
        buildObject.transform.position = mousePosition + new Vector3(0, buildObject.transform.localScale.y * 0.5f, 0);
    }
    private void GetInput()
    {
        if (buildType != BuildType.none && canBuild && Input.GetButtonDown("Fire1"))
        {
            if (buildType == BuildType.turret)
            {
                var newTurret = PhotonNetwork.Instantiate(buildPrefabName, buildObject.transform.position, buildObject.transform.rotation);
                newTurret.transform.SetParent(myHomeArea.transform);
            }

            if (buildType == BuildType.house)
            {
                var newHouse = PhotonNetwork.Instantiate(buildPrefabName, buildObject.transform.position, buildObject.transform.rotation);
                var newHomeArea = PhotonNetwork.Instantiate(homeAreaPrefabName, buildObject.transform.position, buildObject.transform.rotation);
                homeAreas.Add(newHomeArea.gameObject);
            }
            PlayerStat.LocalPlayer.gameObject.GetComponent<PlayerController>().isBuilding = false;

            buildType = BuildType.none;
            Destroy(buildObject);
            buildObject = null;
            foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = false;
        }
        if (buildType != BuildType.none && Input.GetButtonDown("Fire2"))
        {
            PlayerStat.LocalPlayer.gameObject.GetComponent<PlayerController>().isBuilding = false;

            buildType = BuildType.none;
            Destroy(buildObject);
            buildObject = null;
            foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}