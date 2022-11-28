using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundTrigger : MonoBehaviourPun, IPunObservable //¶¥
{
    public TurretController myTurret;
    public bool inOtherHome=false; //private turret

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((bool)inOtherHome);
        }
        else
        {
            this.inOtherHome = (bool)stream.ReceiveNext();
        }
    }

    private void Awake() //change later~~~?~~?~?~?~?~
    {
        this.gameObject.name = photonView.Owner.NickName+"HomeArea";
        //myTurret.trigger = this;
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.name != photonView.Owner.NickName) && myTurret != null && other.gameObject.tag == "Player")
            inOtherHome = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            inOtherHome = false;
    }
}
