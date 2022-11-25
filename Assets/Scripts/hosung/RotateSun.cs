using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RotateSun : MonoBehaviourPun
{
    [Range(0, 23)] public float curTime = 1;
    public string curRealTime;
    public float oneHourPerSeconds = 60 * 60;
    [SerializeField] GameObject directionalLight;
    [SerializeField] GameObject nightDirectionalLight;
    [SerializeField] TextMeshProUGUI timeText;
    private float curSeconds = 0;
    private Color originLightColor;
    [SerializeField] Color nightColor = new Color(0.5f, 0.5f, 0.5f);

    void Awake()
    {
        originLightColor = GetComponent<Light>().color;
        nightDirectionalLight.GetComponent<Light>().color = nightColor;
    }

    [PunRPC]
    public void SetTime(float _curTime, float _curSeconds)
    {
        curTime = _curTime;
        curSeconds = _curSeconds;

        SetAngleAndColor();
    }

    void SetAngleAndColor()
    {
        // float _lightDir = curTime < 23 ? (180 / 23) * (curTime + 1 + (curSeconds == 0 ? 0 : curSeconds / oneHourPerSeconds)) : 180f + 180f * (curSeconds == 0 ? 0 : curSeconds / oneHourPerSeconds);
        float _lightDir = (360 / 24) * (curTime + (curSeconds == 0 ? 0 : curSeconds / oneHourPerSeconds));

        // 180 --> 1, 360 --> 0 (0 ~ 360)
        nightDirectionalLight.GetComponent<Light>().shadowStrength = 1 - (_lightDir > 180 ? (_lightDir == 360 ? 0 : (360 - _lightDir) / 180) : (_lightDir == 0 ? 0 : _lightDir / 180));

        Color _curColor = curTime > 12 ? Color.Lerp(originLightColor, nightColor, (curTime + (curSeconds / oneHourPerSeconds) - 12 == 0 ? 0 : ((curTime + (curSeconds / oneHourPerSeconds) - 12) / 12))) : Color.Lerp(nightColor, originLightColor, (curTime + (curSeconds / oneHourPerSeconds) == 0 ? 0 : ((curTime + (curSeconds / oneHourPerSeconds)) / 12)));
        directionalLight.transform.rotation = Quaternion.Euler(_lightDir - 90f, -90f, 0f);
        directionalLight.GetComponent<Light>().color = _curColor;

        curRealTime = $"{(int)curTime}시 {Mathf.RoundToInt((curSeconds / oneHourPerSeconds) * 60)}분";
        if (timeText != null) timeText.text = curRealTime;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (curSeconds >= oneHourPerSeconds)
            {
                curTime = curTime == 23 ? 0 : curTime + 1;
                curSeconds = 0;
            }
            curSeconds += Time.deltaTime;
            photonView.RPC("SetTime", RpcTarget.AllViaServer, curTime, curSeconds);
        }
    }
}