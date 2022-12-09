using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviourPun
{
    [SerializeField] AudioClip[] backGroundMusic;
    [SerializeField] AudioClip[] stepSounds;
    [SerializeField] AudioClip[] chainsawSounds;
    public AudioSource effectAudioSource;
    public AudioSource otherAudioSource;
    public PlayerStat myCharacterStat;
    public PlayerController myCharacterController;

    bool readyChainsaw = false;

    void Awake()
    {

    }

    public void PlayOneShot(AudioClip _ac, AudioClip _defaultAc, int _actorNum)
    {
        effectAudioSource = PlayerList.Instance.playersWithActorNumber[_actorNum].GetComponents<AudioSource>()[0];
        if (effectAudioSource.clip != _defaultAc)
        {
            effectAudioSource.clip = _defaultAc;
            effectAudioSource.Stop();
            effectAudioSource.PlayOneShot(_ac);
        }
        else
        {
            if (!effectAudioSource.isPlaying)
            {
                effectAudioSource.clip = _defaultAc;
                effectAudioSource.Stop();
                effectAudioSource.PlayOneShot(_ac);
            }
        }
    }

    public void Play(AudioClip _ac, int _actorNum)
    {
        otherAudioSource = PlayerList.Instance.playersWithActorNumber[_actorNum].GetComponents<AudioSource>()[1];
        otherAudioSource.clip = _ac;
        otherAudioSource.Play();
    }

    [PunRPC]
    void OnWalking(int _actorNum)
    {
        PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)], stepSounds[0], _actorNum);
        if (!Input.GetButton("Fire1")) otherAudioSource.Stop();
    }

    [PunRPC]
    void OnAimChainsaw(int _actorNum)
    {
        PlayOneShot(chainsawSounds[0], chainsawSounds[0], _actorNum);
    }

    [PunRPC]
    void OnAttackChainsaw(int _actorNum)
    {
        if (otherAudioSource.isPlaying && otherAudioSource.clip == chainsawSounds[1]) return;
        Play(chainsawSounds[1], _actorNum);
    }

    [PunRPC]
    void StopSounds()
    {
        effectAudioSource.Stop();
        otherAudioSource.Stop();
    }

    void PlaySound()
    {
        if (myCharacterStat == null) return;
        if (myCharacterController == null) myCharacterController = myCharacterStat.GetComponent<PlayerController>();

        if (myCharacterController.moveDirection != Vector3.zero)
        {
            photonView.RPC("OnWalking", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
        }
        else if (!Input.GetButton("Fire1")) otherAudioSource.Stop();

        if (Input.GetButtonDown("Fire2"))
        {
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[2])
            {
                photonView.RPC("OnAimChainsaw", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
            }
        }
        else readyChainsaw = false;

        if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
        {
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[2])
            {
                photonView.RPC("OnAttackChainsaw", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
            }
        }
    }

    void Update()
    {
        PlaySound();
    }
}
