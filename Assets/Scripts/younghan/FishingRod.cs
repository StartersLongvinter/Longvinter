using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform uistin;
    [SerializeField] private Vector3 startRopePoint = new Vector3(0.02563f, -0.000135f, 0.01576f);
    [SerializeField] private Vector3 endRopePointOffset = new Vector3(0, 0, 0.0045f);

    private void FixedUpdate()
    {
        lineRenderer.SetPosition(0, startRopePoint);
        lineRenderer.SetPosition(1, uistin.localPosition + endRopePointOffset);
    }
}
