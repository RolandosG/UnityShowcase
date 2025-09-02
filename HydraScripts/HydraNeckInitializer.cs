using UnityEngine;

public class HydraNeckInitializer : MonoBehaviour
{
    public GameObject hydraBody;

    void Start()
    {
        HydraHeadController headController = GetComponentInChildren<HydraHeadController>();

        if (headController != null && hydraBody != null)
        {
            headController.neckParent = hydraBody.transform;
        }
    }
}
