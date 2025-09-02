using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraHeadController : MonoBehaviour
{
    public Transform neckParent;
    private Transform lastNeckSegment;
    private Vector3 initialOffset;

    void Start()
    {
        if (neckParent != null)
        {
            UpdateLastNeckSegment();
            initialOffset = transform.position - lastNeckSegment.position;
        }
    }

    void Update()
    {
        if (lastNeckSegment != null)
        {
            transform.position = lastNeckSegment.position + initialOffset;
        }
    }

    public void UpdateLastNeckSegment()
    {
        if (neckParent != null && neckParent.childCount > 1)
        {
            lastNeckSegment = neckParent.GetChild(neckParent.childCount - 2);
        }
        else
        {
            lastNeckSegment = null;
        }
    }

}
