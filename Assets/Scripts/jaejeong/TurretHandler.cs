using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Photon.Pun;

public class TurretHandler : MonoBehaviourPun, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    TurretController TurretCollider;
    bool isAuto;
    public void OnPointerClick(PointerEventData eventData)
    {
        TurretCollider = eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>();
        isAuto = !TurretCollider.IsAuto;
        TurretCollider.IsAuto = isAuto;
        TurretCollider.ChangeTurretModeColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid &&
            eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>() != null)
        {
            TurretCollider = eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>();
            if (TurretCollider.turretOwner == "")
                return;
            if((TurretCollider.GetComponent<PhotonView>().Owner.NickName == PhotonNetwork.LocalPlayer.NickName)&&!GetComponent<PlayerController>().IsAiming)
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
        TurretCollider = eventData.pointerCurrentRaycast.gameObject.GetComponent<TurretController>();
        TurretCollider.transform.GetChild(1).gameObject.SetActive(false);
        TurretCollider.transform.GetChild(4).gameObject.SetActive(false);
    }
}