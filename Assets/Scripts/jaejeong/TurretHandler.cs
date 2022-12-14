using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretHandler : MonoBehaviourPun
{
    [SerializeField] Vector3 offset;

    private Camera mainCamera;
    private Camera uiCamera;
    private TurretController turretCollider;
    private Transform turretChildTransform;
    private bool isAuto;

    private void Start()
    {
        mainCamera = Camera.main;
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        turretCollider = GetComponent<TurretController>();
        turretChildTransform = turretCollider.transform.GetChild(2);
        this.GetComponentInChildren<Canvas>().worldCamera = uiCamera;
    }

    void OnMouseOver()
    {
        if (!GetComponent<PhotonView>().IsMine|| turretCollider.turretOwner == "")
            return;

        if ((photonView.Owner.NickName == PhotonNetwork.LocalPlayer.NickName) && PlayerStat.LocalPlayer.status!=PlayerStat.Status.Aim)//!PlayerStat.LocalPlayer.GetComponent<PlayerController>().IsAiming)
        {
            Vector3 _finalPosition = mainCamera.WorldToScreenPoint(transform.position);
            _finalPosition = uiCamera.ScreenToWorldPoint(_finalPosition);
            _finalPosition = new Vector3(_finalPosition.x, _finalPosition.y, 0);
            turretChildTransform.transform.position = _finalPosition + offset;

            if (turretCollider.IsAuto)
                turretChildTransform.GetChild(3).gameObject.SetActive(false);
            else
                turretChildTransform.GetChild(3).gameObject.SetActive(true);
            turretChildTransform.GetChild(4).gameObject.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        isAuto = !turretCollider.IsAuto;
        turretCollider.IsAuto = isAuto;
        turretCollider.ChangeTurretModeColor();
    }

    void OnMouseExit()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        turretChildTransform.GetChild(3).gameObject.SetActive(false);
        turretChildTransform.GetChild(4).gameObject.SetActive(false);
    }
}