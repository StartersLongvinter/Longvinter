using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EffectSound : MonoBehaviourPun
{
    public List<SoundList> soundLists = new List<SoundList>();
    public AudioSource[] audioSource;

    void Awake()
    {
        audioSource = GetComponents<AudioSource>();
    }

    public void PlaySound(string _soundsName, int _index)
    {
        photonView.RPC("RPCPlaySound", RpcTarget.All, _soundsName, _index);
    }

    [PunRPC]
    public void RPCPlaySound(string _soundsName, int _index)
    {
        audioSource[0].PlayOneShot(soundLists.Find(t => t.name == _soundsName).clips[_index]);
    }

    void Update()
    {

    }
}
