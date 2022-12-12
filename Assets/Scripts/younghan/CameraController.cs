using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public enum View { Default, Building, TopDown, Attack }

    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerController playerController;

    [Header("Camera")]
    public CinemachineVirtualCamera[] virtualCameras;
    [SerializeField] private CinemachineBrain brain;

    [Header("Zoom")]
    [SerializeField] private float speed = 1500f;
    [SerializeField] private Vector2 distanceMinMax = new Vector2(11f, 19f);

    [Header("Attack View")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private float addDistance = 6f;
    [SerializeField] private float mousePositionThreshold = 10f;

    private List<CinemachineComponentBase> componentBases = new List<CinemachineComponentBase>();
    private CinemachineBasicMultiChannelPerlin basicMultiChannelPerlin;

    void Start()
    {
        for (int i = 0; i < virtualCameras.Length; i++)
            componentBases.Add(virtualCameras[i].GetCinemachineComponent(CinemachineCore.Stage.Body));

        basicMultiChannelPerlin = virtualCameras[(int)View.Attack].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        SwitchView();
        Zoom();
        FollowMouse();
    }

    public void SwitchView()
    {
        if (playerController == null)
            return;

        // Priority of 'Default' view camera - 10
        if (playerController.IsAiming)
            virtualCameras[(int)View.Attack].Priority = 13;
        else
            virtualCameras[(int)View.Attack].Priority = 7;

        if (Input.GetKey(KeyCode.Q))
            virtualCameras[(int)View.TopDown].Priority = 12;
        else
        {
            virtualCameras[(int)View.TopDown].Priority = 8;

            if (playerController.isBuilding)
                virtualCameras[(int)View.Building].Priority = 11;
            else
                virtualCameras[(int)View.Building].Priority = 9;
        }
    }

    public void Zoom()
    {
        if (playerController == null)
            return;

        if (playerController.IsAiming)
            return;

        float cameraDistance = Input.GetAxis("Mouse ScrollWheel") * speed * Time.deltaTime;

        for (int i = 0; i < componentBases.Count; i++)
        {
            if (componentBases[i] is CinemachineFramingTransposer)
            {
                var framingTransposer = componentBases[i] as CinemachineFramingTransposer;

                framingTransposer.m_CameraDistance -= cameraDistance;

                if (cameraDistance < 0)
                {
                    if (i == (int)View.Attack)
                    {
                        if (framingTransposer.m_CameraDistance > distanceMinMax.y + addDistance)
                            framingTransposer.m_CameraDistance = distanceMinMax.y + addDistance;
                    }
                    else
                    {
                        if (framingTransposer.m_CameraDistance > distanceMinMax.y)
                            framingTransposer.m_CameraDistance = distanceMinMax.y;
                    }
                }
                else
                {
                    if (i == (int)View.Attack)
                    {
                        if (framingTransposer.m_CameraDistance < distanceMinMax.x + addDistance)
                            framingTransposer.m_CameraDistance = distanceMinMax.x + addDistance;
                    }
                    else
                    {
                        if (framingTransposer.m_CameraDistance < distanceMinMax.x)
                            framingTransposer.m_CameraDistance = distanceMinMax.x;
                    }
                }
            }
        }
    }

    public void FollowMouse()
    {
        if (player == null || playerController == null)
            return;

        if (playerController.IsAiming)
        {
            if (playerController.weaponData.eqClassify == EquipmentData.EquipmentClassify.RangeWeapon)
            {
                Vector3 targetPosition = player.position + ((playerController.AimLookPoint - player.position) / 3f);

                targetPosition.x = Mathf.Clamp(targetPosition.x, player.position.x - mousePositionThreshold, player.position.x + mousePositionThreshold);
                targetPosition.z = Mathf.Clamp(targetPosition.z, player.position.z - mousePositionThreshold, player.position.z + mousePositionThreshold);

                followTarget.position = targetPosition;

                virtualCameras[(int)View.Attack].Follow = followTarget;
            }
            else
                virtualCameras[(int)View.Attack].Follow = playerController.cameraFollowTarget;
        }
        else
            virtualCameras[(int)View.Attack].Follow = null;
    }

    public IEnumerator Shake(float amplitudeGain, float frequencyGain, float duration)
    {
        basicMultiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        basicMultiChannelPerlin.m_FrequencyGain = frequencyGain;

        yield return new WaitForSeconds(duration);

        basicMultiChannelPerlin.m_AmplitudeGain = 0f;
        basicMultiChannelPerlin.m_FrequencyGain = 0f;
    }
}
