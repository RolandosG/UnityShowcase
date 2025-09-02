using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public Vector3 direction;
    public float speed = 5.0f;
    private Rigidbody rb;
    public Transform fireTrail;

    private Transform playerTransform;
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        // Make the fireball face the player
        transform.LookAt(playerTransform);

        // Set the direction towards the player
        direction = (playerTransform.position - transform.position).normalized;

        // Destroy the fireball after 5 seconds
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        rb.velocity = direction * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(20);
            }

            // Optionally, you can destroy the fireball after dealing damage
            Destroy(gameObject);
        }
    }
}
