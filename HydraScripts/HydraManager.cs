using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HydraManager : MonoBehaviour
{
    public GameObject hydraNeckPrefab;
    public GameObject hydraChestPrefab;
    
    public Transform hydraBodySpawnPoint;

    public Transform[] spawnPoints;

    private int headsDestroyed = 0;


    public AudioClip hydraSpawnSound;
    protected AudioSource hydraAudioSource;

    void Start()
    {
        hydraAudioSource = GetComponent<AudioSource>();
        if (hydraAudioSource == null)
        {
            hydraAudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (hydraSpawnSound != null)
        {
            hydraAudioSource.PlayOneShot(hydraSpawnSound);
        }
        //GameObject hydraBody = Instantiate(hydraChestPrefab);
        GameObject hydraBody = Instantiate(hydraChestPrefab, hydraBodySpawnPoint.position, Quaternion.identity);
        spawnPoints = hydraBody.GetComponentsInChildren<Transform>().Where(t => t != hydraBody.transform).ToArray();

        Debug.Log("Spawn points: " + string.Join(", ", spawnPoints.Select(sp => sp.position.ToString())));

        // Spawn the hydra body at the spawn point
        //GameObject hydraBody = Instantiate(hydraChestPrefab, hydraBodySpawnPoint.position, Quaternion.identity);

        // Set the parent of the hydra body
        hydraBody.transform.SetParent(transform);

        // Spawn the hydra chest and head/neck
        //SpawnHydraChest();
        SpawnHydraHeadAndNeck(0); // Spawn neck at MiddleHead position
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
            SpawnHydraHeadAndNeck(1); // Spawn neck at LeftHead position
            SpawnHydraHeadAndNeck(2); // Spawn neck at RightHead position
        }
        else if (headsDestroyed == 3)
        {
            SpawnHydraHeadAndNeck(0); // Spawn neck at MiddleHead position
            SpawnHydraHeadAndNeck(1); // Spawn neck at LeftHead position
            SpawnHydraHeadAndNeck(2); // Spawn neck at RightHead position
        }
        else if (headsDestroyed == 6)
        {
            SpawnHydraHeadAndNeck(1); // Spawn neck at LeftHead position
            SpawnHydraHeadAndNeck(2); // Spawn neck at RightHead position
            SpawnHydraHeadAndNeck(3); // Spawn neck at LeftSideHead position
            SpawnHydraHeadAndNeck(4); // Spawn neck at RightSideHead position
        }

    }

    private void SpawnHydraHeadAndNeck(int spawnPointIndex)
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

    private void SpawnHydraChest()
    {
        GameObject hydraBody = GameObject.FindGameObjectWithTag("HydraBody");
        GameObject newChest = Instantiate(hydraChestPrefab, hydraBody.transform);
        newChest.transform.localPosition = Vector3.zero; // Adjust this position as needed
    }




}
