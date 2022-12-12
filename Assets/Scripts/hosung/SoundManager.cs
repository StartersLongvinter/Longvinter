using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class SoundManager : MonoBehaviourPun
{
    #region Create Singleton
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj;
                obj = GameObject.Find("SoundManager");
                if (obj == null)
                {
                    obj = new GameObject("SoundManager");
                    obj.AddComponent<PhotonView>();
                    instance = obj.AddComponent<SoundManager>();
                }
                else
                {
                    obj.AddComponent<SoundManager>();
                    instance = obj.GetComponent<SoundManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    SoundList backgroundMusics;

    #region Init
    void Awake()
    {
        if (instance == null) instance = this;

        NetworkManager.Instance.GetComponent<AudioSource>().Stop();
        backgroundMusics = NetworkManager.Instance.backgroundMusics;
    }
    #endregion

    #region Public call functions
    public void PlayPlayerSound(string _soundsName, int _soundIdx, bool _isPlayOneShot = true, bool _isWaitDealy = false)
    {
        int _playerIndex = PlayerStat.LocalPlayer.ownerPlayerActorNumber;
        photonView.RPC("RPCPlayPlayerSound", RpcTarget.All, _playerIndex, _soundsName, _soundIdx, _isPlayOneShot, _isWaitDealy);
    }

    public void PlayToolSound(string _soundsName, int _soundIdx, bool _isPlayOneShot = true, bool _isWaitDealy = false)
    {
        int _playerIndex = PlayerStat.LocalPlayer.ownerPlayerActorNumber;
        photonView.RPC("RPCPlayToolSound", RpcTarget.All, _playerIndex, _soundsName, _soundIdx, _isPlayOneShot, _isWaitDealy);
    }

    public void PlaySoundOnObject(GameObject go, string _soundsName, int _soundIdx)
    {
        go.GetComponent<EffectSound>().PlaySound(_soundsName, _soundIdx);
    }

    public void PlayBackgroundMusic(int _bgmIndex)
    {
        AudioSource _audioSource = NetworkManager.Instance.GetComponent<AudioSource>();
        _audioSource.Stop();
        _audioSource.clip = backgroundMusics.clips[_bgmIndex];
        _audioSource.Play();
    }
    #endregion

    #region RPC functions
    [PunRPC]
    public void RPCPlayPlayerSound(int _playerIndex, string _soundsName, int _soundIdx, bool _isPlayOneShot, bool _isWaitDealy)
    {
        EffectSound _effectSound = PlayerList.Instance.playersWithActorNumber[_playerIndex].GetComponent<EffectSound>();
        AudioSource playerSoundSource = _effectSound.audioSource[0];
        SoundList _soundList = _effectSound.soundLists.Find(t => t.name == _soundsName);
        AudioClip _ac = _soundList.clips[_soundIdx == -1 ? Random.Range(0, _soundList.clips.Length - 1) : _soundIdx];

        switch (_isPlayOneShot)
        {
            case true:
                if (_isWaitDealy)
                {
                    if (!playerSoundSource.isPlaying)
                        playerSoundSource.PlayOneShot(_ac);
                }
                else playerSoundSource.PlayOneShot(_ac);
                break;
            case false:
                if (playerSoundSource.clip != _ac)
                {
                    playerSoundSource.Stop();
                    playerSoundSource.clip = _ac;
                    playerSoundSource.Play();
                }
                break;
        }
    }

    [PunRPC]
    public void RPCPlayToolSound(int _playerIndex, string _soundsName, int _soundIdx, bool _isPlayOneShot, bool _isWaitDealy)
    {
        EffectSound _effectSound = PlayerList.Instance.playersWithActorNumber[_playerIndex].GetComponent<EffectSound>();
        AudioSource toolSoundSource = _effectSound.audioSource[0];
        SoundList _soundList = _effectSound.soundLists.Find(t => t.name == _soundsName);
        AudioClip _ac = _soundList.clips[_soundIdx == -1 ? Random.Range(0, _soundList.clips.Length - 1) : _soundIdx];

        switch (_isPlayOneShot)
        {
            case true:
                if (_isWaitDealy)
                {
                    if (!toolSoundSource.isPlaying)
                        toolSoundSource.PlayOneShot(_ac);
                }
                else toolSoundSource.PlayOneShot(_ac);
                break;
            case false:
                if (toolSoundSource.clip != _ac)
                {
                    toolSoundSource.Stop();
                    toolSoundSource.clip = _ac;
                    toolSoundSource.Play();
                }
                break;
        }
    }
    #endregion
}
