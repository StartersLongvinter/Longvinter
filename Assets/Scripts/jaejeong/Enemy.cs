using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    [HideInInspector]
    public float speed;

    public static Color originalColor;
    private SkinnedMeshRenderer renderer;
    public bool isChanged = false;

    [PunRPC]
    public void ChangePlayersColor(float damage)
    {
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (photonView.IsMine)
        {
            Debug.Log(photonView.Owner.NickName);
            PlayerStat.LocalPlayer.currentHPAnimator.SetTrigger("isDamaged");
            PlayerStat.LocalPlayer.ChangeHp(-1f * damage);
            PlayerStat.LocalPlayer.ChangeStatus((int)PlayerStat.Status.damaged);
        }
        StartCoroutine(ResetColor());
    }

    IEnumerator ResetColor()
    {
        if (renderer.material.color != Color.red)
        {
            originalColor = renderer.material.color;
            isChanged = true;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            renderer.material.color = originalColor;
            isChanged = false;
            GetComponent<PlayerStat>().ChangeStatus((int)PlayerStat.Status.idle);
        }
        yield return null;
    }
}
