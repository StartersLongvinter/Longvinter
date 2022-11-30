using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAnimator : MonoBehaviour
{
    void Awake()
    {

    }

    public void DamagedAnimOn()
    {
        PlayerStat.LocalPlayer.currentHPAnimator.SetBool("isAnimate", true);
    }

    public void DamagedAnimOff()
    {
        PlayerStat.LocalPlayer.currentHPAnimator.SetBool("isAnimate", false);
    }

    void Update()
    {

    }
}
