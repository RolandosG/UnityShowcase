using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadManager : MonoBehaviour
{
    public GameObject hydraHeadPrefab;
    public HydraBodyController bodyController;

    private List<Transform> headSpawnPoints = new List<Transform>();
    private List<GameObject> heads = new List<GameObject>();

    void Start()
    {
        // Get all of the head spawn points on the HydraBody
        foreach (Transform child in bodyController.transform)
        {
            if (child.CompareTag("HeadSpawnPoint"))
            {
                headSpawnPoints.Add(child);
            }
        }

        // Spawn the initial head
        SpawnHead(0);
    }

    private void SpawnHead(int spawnPointIndex)
    {
        // Instantiate the new head without setting the parent
        GameObject newHead = Instantiate(hydraHeadPrefab, Vector3.zero, Quaternion.identity);

        // Set the parent and position of the new head manually
        newHead.transform.SetParent(headSpawnPoints[spawnPointIndex]);
        newHead.transform.localPosition = Vector3.zero;

        // Add the new head to the list of heads
        heads.Add(newHead);

        // Subscribe to the OnHeadDestroyed event of the HydraHeadController
        HydraNeckController headController = newHead.GetComponent<HydraNeckController>();
       // headController.OnHeadDestroyed += HandleHeadDestroyed;
    }

    private void HandleHeadDestroyed(GameObject destroyedHead)
    {
        // Unsubscribe from the OnHeadDestroyed event of the destroyed head
        HydraNeckController NeckController = destroyedHead.GetComponent<HydraNeckController>();
        //NeckController.OnHeadDestroyed -= HandleHeadDestroyed;

        // Remove the destroyed head from the list of heads
        heads.Remove(destroyedHead);

        // If all heads have been destroyed, end the game
        if (heads.Count == 0)
        {
            Debug.Log("All heads destroyed. Game over.");
            // TODO: End the game
            return;
        }

        // Determine which spawn point to use for the new head
        int newSpawnPointIndex = Random.Range(0, headSpawnPoints.Count);

        // Spawn the new head
        SpawnHead(newSpawnPointIndex);
    }

}
