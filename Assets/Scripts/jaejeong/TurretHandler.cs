using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Photon.Pun;

public class TurretHandler : MonoBehaviour
{
    TurretController TurretCollider;
    bool isAuto;

    void OnMouseEnter()
    {
        Debug.Log("???? Enter");
    }

    void OnMouseOver()
    {
        Debug.Log("???? Over");
    }

    void OnMouseExit()
    {
        Debug.Log("???? Exit");
    }
/*    public void OnPointerClick(PointerEventData eventData)
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
            if ((TurretCollider.GetComponent<PhotonView>().Owner.NickName == PhotonNetwork.LocalPlayer.NickName)&&!GetComponent<PlayerController>().IsAiming)
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