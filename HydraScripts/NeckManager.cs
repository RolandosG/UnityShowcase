using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckManager : MonoBehaviour
{
    public GameObject hydraNeckPrefab;

    private BodyManager bodyManager; // reference to the BodyManager script

    void Start()
    {
        // Get the BodyManager component from the scene
        bodyManager = FindObjectOfType<BodyManager>();
    }

    public void SpawnRemainingNecks(Transform[] spawnPoints)
    {
        // Spawn the remaining necks
        for (int i = 3; i < spawnPoints.Length; i++)
        {
            SpawnNeck(spawnPoints[i]);
        }
    }

    private void SpawnNeck(Transform spawnPoint)
    {
        // Instantiate the new neck without setting the parent
        GameObject newNeck = Instantiate(hydraNeckPrefab, Vector3.zero, Quaternion.identity);

        // Set the parent and position of the new neck manually
        newNeck.transform.SetParent(spawnPoint);
        newNeck.transform.localPosition = Vector3.zero;

        // Debug lines to check for null references and positions
        Debug.Log("newNeck: " + newNeck);

        Debug.Log("newNeck position: " + newNeck.transform.position);
    }
}
