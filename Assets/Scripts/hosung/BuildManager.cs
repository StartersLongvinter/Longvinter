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
    [SerializeField] public string[] buildPrefabNameList;
    List<GameObject> homeAreas = new List<GameObject>();
    [SerializeField] public string homeAreaPrefabName;

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
            foreach (GameObject area in buildArea)
            {
                if (area.name == PhotonNetwork.LocalPlayer.NickName + "HomeArea" && !myHomeAreas.Contains(area)) myHomeAreas.Add(area);
            }
        if (buildType == BuildType.turret)
        {
            if (myHomeAreas.Count <= 0 || Vector3.Distance(PlayerStat.LocalPlayer.gameObject.transform.position, _mousePosition) > 4f)
            {
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
            if (_d > (myHomeArea.transform.lossyScale.x * 0.5f) - buildObject.transform.localScale.x * 0.5f)
            {
                return false;
            }
        }
        else if (buildType == BuildType.house)
        {
            if (Vector3.Distance(PlayerStat.LocalPlayer.gameObject.transform.position, _mousePosition) > 4f)
            {
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
            if (_distance <= homeAreaRadius)
            {
                return false;
            }
        }
        return true;
    }

    void CheckRayCast()
    {
        if (buildType == BuildType.none) return;

        RaycastHit hit;
        Vector3 mouseVector = Input.mousePosition;
        mouseVector.z = Camera.main.farClipPlane;
        Vector3 dir = Camera.main.ScreenToWorldPoint(mouseVector);
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);
        Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red);
        // int layerMask = 1 << LayerMask.NameToLayer(��Player��) | 1 << LayerMask.NameToLayer(��Area��);
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("Area"));
        // 정중앙 검출
        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = CheckBuildPosition(hit.point);
            }
            // if (hit.transform.gameObject != buildObject)
            mousePosition = hit.point;
        }
        // 위 가운데
        Vector3 _offsetY = new Vector3(0, 0, buildObject.transform.lossyScale.z * 0.5f);
        if (canBuild && Physics.Raycast(Camera.main.transform.position + _offsetY, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 아래 가운데 
        if (canBuild && Physics.Raycast(Camera.main.transform.position - _offsetY, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 왼쪽 가운데 
        Vector3 _offsetX = new Vector3(buildObject.transform.lossyScale.x * 0.5f, 0, 0);
        if (canBuild && Physics.Raycast(Camera.main.transform.position - _offsetX, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 우측 가운데 
        if (canBuild && Physics.Raycast(Camera.main.transform.position + _offsetX, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 상단 좌측 
        if (canBuild && Physics.Raycast(Camera.main.transform.position - _offsetX + _offsetY, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 상단 우측
        if (canBuild && Physics.Raycast(Camera.main.transform.position + _offsetX + _offsetY, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 하단 좌측
        if (canBuild && Physics.Raycast(Camera.main.transform.position - _offsetX - _offsetY, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        // 하단 우측 
        if (canBuild && Physics.Raycast(Camera.main.transform.position + _offsetX - _offsetY, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player" || hit.transform.gameObject.tag == "InstalledObject")
            {
                canBuild = false;
            }
            else
            {
                canBuild = true;
            }
        }
        Debug.DrawRay(ray.origin + _offsetX, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin - _offsetX, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin + _offsetY, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin - _offsetY, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin + _offsetX + _offsetY, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin + _offsetX - _offsetY, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin - _offsetX + _offsetY, ray.direction * 500f, Color.red);
        Debug.DrawRay(ray.origin - _offsetX - _offsetY, ray.direction * 500f, Color.red);
        buildObject.transform.position = mousePosition + new Vector3(0, buildObject.transform.localScale.y * 0.5f, 0);

        if (!canBuild)
        {
            if (buildType == BuildType.turret)
            {
                foreach (Material mat in buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials)
                {
                    mat.color = buildObjectColors[1];
                }

                foreach (Material mat in buildObject.transform.GetChild(1).GetComponent<MeshRenderer>().materials)
                {
                    mat.color = buildObjectColors[1];
                }
            }
            else
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
        }
        else
        {
            if (buildType == BuildType.turret)
            {
                foreach (Material mat in buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials)
                {
                    mat.color = buildObjectColors[0];
                }

                foreach (Material mat in buildObject.transform.GetChild(1).GetComponent<MeshRenderer>().materials)
                {
                    mat.color = buildObjectColors[0];
                }
            }
            else
                buildObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = buildObjectColors[0];
        }
    }

    void Update()
    {
        GetInput();
        if (buildType == BuildType.none) return;
        buildArea = GameObject.FindGameObjectsWithTag("Area");
        foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = true;

        CheckRayCast();
    }
    private void GetInput()
    {
        if (buildType != BuildType.none && canBuild && Input.GetButtonDown("Fire1"))
        {
            if (buildType == BuildType.turret)
            {
                var newTurret = PhotonNetwork.Instantiate(buildPrefabName, buildObject.transform.position, buildObject.transform.rotation);
                newTurret.transform.SetParent(myHomeArea.transform);
                newTurret.GetComponent<TurretController>().turretOwner = PhotonNetwork.LocalPlayer.NickName;
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
            myHomeArea = null;
            foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = false;
        }
        if (buildType != BuildType.none && Input.GetButtonDown("Fire2"))
        {
            PlayerStat.LocalPlayer.gameObject.GetComponent<PlayerController>().isBuilding = false;

            buildType = BuildType.none;
            Destroy(buildObject);
            buildObject = null;
            myHomeArea = null;
            foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}