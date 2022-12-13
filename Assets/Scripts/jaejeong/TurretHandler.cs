using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Photon.Pun;

public class TurretHandler : MonoBehaviourPun
{
    TurretController turretCollider;
    Transform turretChildTransform;
    bool isAuto;

    private void Start()
    {
        turretCollider = GetComponent<TurretController>();
        turretChildTransform = turretCollider.transform.GetChild(2);
    }

    void OnMouseOver()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (turretCollider.turretOwner == "")
            return;

        if ((photonView.Owner.NickName == PhotonNetwork.LocalPlayer.NickName) && PlayerStat.LocalPlayer.status!=PlayerStat.Status.Aim)//!PlayerStat.LocalPlayer.GetComponent<PlayerController>().IsAiming)
        {
            //turretChildTransform.LookAt(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
            var lookPos = turretChildTransform.position - Camera.main.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            turretChildTransform.rotation = rotation;
            if (turretCollider.IsAuto)
            {
                turretChildTransform.GetChild(4).gameObject.SetActive(true);
                turretChildTransform.GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                turretChildTransform.GetChild(3).gameObject.SetActive(true);
                turretChildTransform.GetChild(4).gameObject.SetActive(true);
            }
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
    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>() == null)
            return;
        TurretCollider = eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>();
        isAuto = !TurretCollider.IsAuto;
        TurretCollider.IsAuto = isAuto;
        TurretCollider.ChangeTurretModeColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("?3");
        if (eventData.pointerCurrentRaycast.isValid &&
            eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>() != null)
        {
            Debug.Log("?1");
            TurretCollider = eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>();
            if (TurretCollider.turretOwner == "")
                return;

            Debug.Log("?2");
            if ((TurretCollider.GetComponent<PhotonView>().Owner.NickName == PhotonNetwork.LocalPlayer.NickName) && !GetComponent<PlayerController>().IsAiming)
            {
                if (TurretCollider.IsAuto)
                {
                    TurretCollider.transform.GetChild(1).gameObject.SetActive(true);
                    TurretCollider.transform.GetChild(4).gameObject.SetActive(false);
                }
                else
                {
                    TurretCollider.transform.GetChild(1).gameObject.SetActive(true);
                    TurretCollider.transform.GetChild(4).gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>() == null)
            return;
        TurretCollider = eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>();
        TurretCollider.transform.GetChild(1).gameObject.SetActive(false);
        TurretCollider.transform.GetChild(4).gameObject.SetActive(false);
    }*/
}