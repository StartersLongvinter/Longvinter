using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum BuildType
{
    none = 0,
    turret,
    house,
    other,
    countIndex // don't set this type! This type only use to check enum's length.
}

public class BuildManager : MonoBehaviour
{
    BuildType buildType = BuildType.none;
    public Vector3 mousePosition;
    [SerializeField] GameObject[] buildObjectPrefab;
    GameObject buildObject;
    public bool canBuild = true;

    GameObject[] buildArea;
    [SerializeField] Color[] buildObjectColors;

    GameObject myHomeArea;

    void Awake()
    {

    }

    public void SetBuildType(int buildtypeNumber)
    {
        buildType = (BuildType)buildtypeNumber;
        buildObject = Instantiate(buildObjectPrefab[(buildtypeNumber)], Vector3.zero, Quaternion.identity);
    }

    bool CheckBuildPosition(Vector3 _mousePosition)
    {
        if (myHomeArea == null)
            foreach (GameObject area in buildArea) if (area.name == "MyHomeArea") myHomeArea = area;

        if (buildType == BuildType.turret)
        {
            if (myHomeArea == null || Vector3.Distance(PlayerStat.LocalPlayer.gameObject.transform.position, _mousePosition) > 4f)
            {
                buildObject.GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                return false;
            }

            float _distance = Vector3.Distance(myHomeArea.transform.position, _mousePosition);
            if (_distance > (myHomeArea.transform.lossyScale.x * 0.5f) - buildObject.transform.localScale.x * 0.5f)
            {
                buildObject.GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
                return false;
            }
        }

        buildObject.GetComponent<MeshRenderer>().material.color = buildObjectColors[0];
        return true;
    }

    void Update()
    {
        if (buildType == BuildType.none) return;

        buildArea = GameObject.FindGameObjectsWithTag("Area");
        foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = true;

        // mousePosition = GameObject.Find("UICamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        RaycastHit hit;
        Vector3 mouseVector = Input.mousePosition;
        mouseVector.z = Camera.main.farClipPlane;
        Vector3 dir = Camera.main.ScreenToWorldPoint(mouseVector);
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);
        Debug.DrawRay(ray.origin, ray.direction * 500f, Color.red);

        // int layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Area");
        int layerMask = (-1) - (1 << LayerMask.NameToLayer("Area"));

        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, 500f, layerMask))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                buildObject.GetComponent<MeshRenderer>().material.color = buildObjectColors[1];
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

    private void FixedUpdate()
    {
        if (buildType != BuildType.none && canBuild && Input.GetMouseButtonDown(0))
        {
            PhotonNetwork.Instantiate(buildType.ToString(), buildObject.transform.position, buildObject.transform.rotation);
            buildType = BuildType.none;
            buildObject = null;

            foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = false;
        }
        // else if (buildType != BuildType.none && canBuild && Input.GetMouseButtonDown(1))
        // {
        //     buildType = BuildType.none;
        //     buildObject = null;

        //     foreach (GameObject area in buildArea) area.GetComponent<MeshRenderer>().enabled = false;
        // }
    }
}
