using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraHeadHealth : MonoBehaviour
{
    public int health = 5;
    public GameObject hydraHeadPrefab;
    public GameObject hydraNeckPrefab;
    public GameObject explosionPrefab;
    //public GameObject hydraBody;
    public BodyManager bodyManager; // reference to the BodyManager script
    private HydraHeadController headController;
    public HydraManager hydraManager;


    void Start()
    {

        headController = GetComponent<HydraHeadController>();
        bodyManager = FindObjectOfType<BodyManager>();
        if (bodyManager.GetComponent<HydraBodyController>() == null)
        {
            bodyManager.gameObject.AddComponent<HydraBodyController>();
        }

    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyHeadAndSpawnTwo();
        }
    }

    private void DestroyHeadAndSpawnTwo()
    {
        // Notify the BossManager that a head was destroyed
        bodyManager.HeadDestroyed();
        

        // ... (the existing code) ...
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        bodyManager.GetComponent<HydraBodyController>()?.HeadDestroyed();


    }





    void Update()
    {
        // Move the head down when all neck cubes are destroyed
        if (headController != null && headController.neckParent.transform.childCount == 0)
        {
            MoveHeadDown();
        }
    }

    void MoveHeadDown()
    {
        float targetYPosition = 16.0f;
        float moveSpeed = 1.0f;
        Vector3 targetPosition = new Vector3(transform.position.x, targetYPosition, transform.position.z);

        if (transform.position.y > targetYPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
