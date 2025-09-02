using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BodyManager : Enemy
{
    public GameObject hydraBodyPrefab;
    public Transform spawnPoint;
    public Transform hydraBodySpawnPoint;
    private GameObject hydraBody;
    public Transform[] spawnPoints;
    public GameObject hydraNeckPrefab;
    public GameObject bodySpawnPointObject;
    public GameObject massiveExplosionPrefab;
    public Vector3 spawnPosition;
    public Vector3 finalPosition;

    public int headsDestroyed = 0;

    public GameObject explosionPrefab;
    public float shakeDuration = 2.5f;

    private NeckManager neckManager;

    void Start()
    {
        // Instantiate the hydra body at the spawn point
        hydraBody = Instantiate(hydraBodyPrefab, spawnPosition, Quaternion.identity);
        hydraBody.transform.SetParent(transform);

        // Get the NeckManager component from the scene
        neckManager = FindObjectOfType<NeckManager>();

        // Get the spawn points from the hydra chest
        Transform hydraChest = hydraBody.transform.Find("HydraChest");
        spawnPoints = hydraChest.GetComponentsInChildren<Transform>().Where(t => t != hydraChest).ToArray();

        // Spawn the initial neck
        SpawnInitialNeck();

        // Move the hydra body to its final position over 2 seconds
        StartCoroutine(MoveToPosition(finalPosition, 2.0f));

    }

    private void SpawnInitialNeck()
    {
        SpawnNeck(0);
    }

    private void SpawnHydraBody()
    {
        if (bodySpawnPointObject != null)
        {
            hydraBody = Instantiate(hydraBodyPrefab, spawnPosition, Quaternion.identity);
            hydraBody.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning("BodySpawnPointObject not set in BodyManager. Spawning body at (0, 0, 0).");
            hydraBody = Instantiate(hydraBodyPrefab, Vector3.zero, Quaternion.identity);
        }
        hydraBody.transform.SetParent(transform);
    }

    public IEnumerator SpawnRemainingNecks(int numNecksToSpawn, int headsDestroyed)
    {
        // Spawn the remaining necks
        for (int i = 1; i < numNecksToSpawn * headsDestroyed + 1; i++)
        {
            SpawnNeck(i);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnNeck(int spawnPointIndex)
    {
        // Instantiate the new neck without setting the parent
        GameObject newNeck = Instantiate(hydraNeckPrefab, Vector3.zero, Quaternion.identity);

        // Set the parent and position of the new neck manually
        newNeck.transform.SetParent(spawnPoints[spawnPointIndex]);
        newNeck.transform.localPosition = Vector3.zero;

        // Debug lines to check for null references and positions
        Debug.Log("newNeck: " + newNeck);
        Debug.Log("newNeck position: " + newNeck.transform.position);
    }

    public void SetSpawnPoints(Transform[] newSpawnPoints)
    {
        spawnPoints = newSpawnPoints;
    }
    public void HeadDestroyed()
    {
        headsDestroyed++;

        if (headsDestroyed == 1)
        {
            SpawnHydraHeadAtNeck(1); // Spawn neck at LeftHead position
            SpawnHydraHeadAtNeck(2); // Spawn neck at RightHead position
        }
        else if (headsDestroyed == 3)
        {
            SpawnHydraHeadAtNeck(0); // Spawn neck at MiddleHead position
            SpawnHydraHeadAtNeck(1); // Spawn neck at LeftHead position
            SpawnHydraHeadAtNeck(2); // Spawn neck at RightHead position
        }
        else if (headsDestroyed == 6)
        {
            SpawnHydraHeadAtNeck(0); // Spawn neck at MiddleHead position
            SpawnHydraHeadAtNeck(1); // Spawn neck at LeftHead position
            SpawnHydraHeadAtNeck(2); // Spawn neck at RightHead position
            SpawnHydraHeadAtNeck(3); // Spawn neck at SideLeftHead position
            SpawnHydraHeadAtNeck(4); // Spawn neck at SideRightHead position
        }
        // Add any other conditions for additional head spawns here
    }
    public void SpawnHydraHeadAtNeck(int spawnPointIndex)
    {
        // Instantiate the new neck without setting the parent
        GameObject newNeck = Instantiate(hydraNeckPrefab, Vector3.zero, Quaternion.identity);

        // Set the parent and position of the new neck manually
        newNeck.transform.SetParent(spawnPoints[spawnPointIndex]);
        newNeck.transform.localPosition = Vector3.zero;

    }
    public IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        float elapsedTime = 0;
        Vector3 startingPos = hydraBody.transform.position;

        while (elapsedTime < duration)
        {
            hydraBody.transform.position = Vector3.Lerp(startingPos, target, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure the hydra body ends up at the exact target position
        hydraBody.transform.position = target;
    }
    private IEnumerator Shake()
    {
        Vector3 originalPosition = hydraBody.transform.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * 0.2f;
            float y = Random.Range(-1f, 1f) * 0.2f;
            hydraBody.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hydraBody.transform.position = originalPosition;
    }
    private IEnumerator Explode()
    {
        foreach (Transform child in hydraBody.transform)
        {
            for (int i = 0; i < 10; i++) // Create 5 explosions per child
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                Vector3 explosionPosition = child.position + randomOffset;
                Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void MassiveExplosion()
    {
        // Instantiate the massive explosion at the Hydra's position
        Instantiate(massiveExplosionPrefab, hydraBody.transform.position, Quaternion.identity);
    }

    public IEnumerator Die()
    {
        // Detach all children (the necks, body, etc.) from the parent hydra object
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).parent = null;
        }

        // Start shaking and exploding
        StartCoroutine(Shake());
        StartCoroutine(Explode());

        yield return new WaitForSeconds(3.0f);

        // Create a massive explosion just before the Hydra gets destroyed
        MassiveExplosion();

        // Stop all coroutines before destroying the hydra body
        StopAllCoroutines();
        base.Defeat();
        // Destroy the main hydra object
        Destroy(hydraBody);
    }



}
