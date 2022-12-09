using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviourPun
{
    [SerializeField] AudioClip[] backGroundMusic;
    [SerializeField] AudioClip[] stepSounds;
    [SerializeField] AudioClip[] chainsawSounds;
    [SerializeField] AudioClip[] shotGunSounds;
    [SerializeField] AudioClip[] rifleSounds;
    [SerializeField] AudioClip gunAimSound;
    [SerializeField] AudioClip notAmmoSound;
    public AudioSource effectAudioSource;
    public AudioSource otherAudioSource;
    public PlayerStat myCharacterStat;
    public PlayerController myCharacterController;

    bool readyChainsaw = false;

    void Awake()
    {

    }

    public void PlayPlayerSound(AudioClip _ac, int _actorNum, AudioClip _defaultAc = null)
    {
        effectAudioSource = PlayerList.Instance.playersWithActorNumber[_actorNum].GetComponents<AudioSource>()[0];
        if (_defaultAc == null)
        {
            effectAudioSource.clip = _ac;
            effectAudioSource.Play();
        }
        else
        {
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
    }

    public void PlayToolSound(AudioClip _ac, int _actorNum, AudioClip _defaultAc = null)
    {
        otherAudioSource = PlayerList.Instance.playersWithActorNumber[_actorNum].GetComponents<AudioSource>()[1];
        if (_defaultAc == null)
        {
            otherAudioSource.clip = _ac;
            otherAudioSource.Play();
        }
        else
        {
            if (otherAudioSource.clip != _defaultAc)
            {
                otherAudioSource.clip = _defaultAc;
                otherAudioSource.Stop();
                otherAudioSource.PlayOneShot(_ac);
            }
            else
            {
                if (!otherAudioSource.isPlaying)
                {
                    otherAudioSource.clip = _defaultAc;
                    otherAudioSource.Stop();
                    otherAudioSource.PlayOneShot(_ac);
                }
            }
        }
    }

    [PunRPC]
    void OnWalking(int _actorNum)
    {
        PlayPlayerSound(stepSounds[Random.Range(0, stepSounds.Length)], _actorNum, stepSounds[0]);
    }

    [PunRPC]
    void OnAimChainsaw(int _actorNum)
    {
        PlayToolSound(chainsawSounds[0], _actorNum, chainsawSounds[0]);
    }

    [PunRPC]
    void OnAimGun(int _actorNum)
    {
        PlayToolSound(gunAimSound, _actorNum, gunAimSound);
    }

    [PunRPC]
    void OnAttackChainsaw(int _actorNum)
    {
        if (otherAudioSource.isPlaying && otherAudioSource.clip == chainsawSounds[1]) return;
        PlayToolSound(chainsawSounds[1], _actorNum);
    }

    [PunRPC]
    void OnAttackGun(int _actorNum, string _gunType)
    {
        switch (_gunType)
        {
            case ("shotGun"):
                PlayToolSound(shotGunSounds[Random.Range(0, shotGunSounds.Length)], _actorNum, shotGunSounds[1]);
                break;
            case ("rifle"):
                PlayToolSound(rifleSounds[Random.Range(0, rifleSounds.Length)], _actorNum, rifleSounds[1]);
                break;
        }
    }

    [PunRPC]
    void StopSounds(int _type = 2)
    {
        // 0 : stop gun sound 1 : stop player sound 2 : stop all sound
        if (_type != 0) effectAudioSource.Stop();
        if (_type != 1) otherAudioSource.Stop();
    }

    void PlaySound()
    {
        if (myCharacterStat == null) return;
        if (myCharacterController == null) myCharacterController = myCharacterStat.GetComponent<PlayerController>();

        if (myCharacterController.moveDirection != Vector3.zero)
        {
            photonView.RPC("OnWalking", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[2])
            {
                photonView.RPC("OnAimChainsaw", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
            }
            if (myCharacterController.weaponData != myCharacterController.weaponDatas[2])
            {
                photonView.RPC("OnAimGun", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
            }
        }
        else readyChainsaw = false;

        if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
        {
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[2])
            {
                photonView.RPC("OnAttackChainsaw", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber);
                return;
            }
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[3])
            {
                photonView.RPC("OnAttackGun", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber, "shotGun");
                return;
            }
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[4])
            {
                photonView.RPC("OnAttackGun", RpcTarget.All, myCharacterStat.ownerPlayerActorNumber, "rifle");
                return;
            }
        }
        else if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
        {
            if (myCharacterController.weaponData == myCharacterController.weaponDatas[2])
                photonView.RPC("StopSounds", RpcTarget.All, 0);
        }
    }

    void Update()
    {
        PlaySound();
    }
}
