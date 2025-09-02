using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject catchPrefab;
    public GameObject enemyRedPrefab;


    public Vector2 spawnRangeX;
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating(nameof(SpawnEvader), 0, 5.50f);
        //InvokeRepeating(nameof(SpawnCatcher), 1.0f, 2.0f);
        //InvokeRepeating(nameof(SpawnRedEvader), 1.0f, 2.5f);
      
       
    }
    private void SpawnCatcher()
    {
        SpawnEnemy(EnemyType.Catcher);
    }
    private void SpawnEvader()
    {
        SpawnEnemy(EnemyType.Evader);
    }
    private void SpawnRedEvader()
    {
        SpawnEnemy(EnemyType.RedEvader);
    }


    // spawns enemy randomly between range x & range x.
    private void SpawnEnemy(EnemyType enemyType)
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnRangeX[0], spawnRangeX[1]),
            enemyPrefab.transform.position.y,
            enemyPrefab.transform.position.z);

        if (enemyType == EnemyType.Evader && enemyPrefab != null)
        {
            Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);
        }
        else if (enemyType == EnemyType.RedEvader && enemyRedPrefab != null)
        {
            Instantiate(enemyRedPrefab, spawnPosition, enemyRedPrefab.transform.rotation);
        }
        else if (catchPrefab != null)
        {
            Instantiate(catchPrefab, spawnPosition, catchPrefab.transform.rotation);
        }

    }

}