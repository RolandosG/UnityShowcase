using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : Enemy
{
    public float health = 100f;  // Splitter's health
    public float damagePerHit = 10f;  // Damage taken from a single projectile

    public GameObject splitterPrefab;  // The Splitter prefab (itself)
    public float size = 4f;  // Current size of the Splitter

    public float speed = 6f;  // The speed of the Splitter
    public float amplitude = 20f;  // The amplitude of the zig-zag movement (maximum X distance from the center)
    private Vector3 center;  // The center of the zig-zag movement

    public float decreaseSpeed = 0.5f;  // The speed at which the spiral should turn into a line
    private float targetAmplitude = 0;  // The target amplitude value
    public float launchSpeed = 20f;  // The speed at which the Splitter will move towards the player once it stops spiraling
    private bool spiraling = true;

    public GameObject explosionPrefab;

    void Start()
    {
        transform.localScale = new Vector3(4f, 4f, 4f);
        StartCoroutine(StartSpiraling());
    }
    IEnumerator StartSpiraling()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        spiraling = false; // Stop the spiraling
    }
    void Update()
    {
        // Add rotation to the Splitter
        transform.Rotate(new Vector3(0, 0, 100 * Time.deltaTime), Space.Self);

        // If the Splitter is still spiraling
        if (spiraling)
        {
            // Calculate the new position
            float theta = speed + 40 * Time.time; // Angle (in radians) changes over time
            amplitude = Mathf.Lerp(amplitude, targetAmplitude, decreaseSpeed * Time.deltaTime); // Decrease the amplitude over time

            // When the amplitude reaches near 0, set the speed to the launch speed
            if (amplitude < 0.4f && speed < launchSpeed)
            {
                speed = launchSpeed;
            }

            float newX = center.x + amplitude * Mathf.Sin(theta); // Spiral X position
            float newY = center.y + amplitude * Mathf.Cos(theta); // Spiral Y position
            float newZ = transform.position.z - speed * Time.deltaTime; // Move towards negative Z

            // Update the Splitter's position
            transform.position = new Vector3(newX, newY, newZ);
        }
        else
        {
            speed = 25;
            // Move towards negative Z
            float newZ = transform.position.z - speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
        }

        // Check if the Splitter's position in Z has passed -90
        if (transform.position.z < -90f)
        {
            Explode();
        }
    }

    public void SetCenter(Vector3 center)
    {
        this.center = center;
    }

    public void TakeDamage(float damage)
    {
        health -= damage; // Decrease the health by the damage amount

        if (health <= 60 && transform.localScale == new Vector3(2f, 2f, 2f))
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
        else if (health <= 100 && transform.localScale == new Vector3(3f, 3f, 3f))
        {
            transform.localScale = new Vector3(2f, 2f, 2f);
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
        else if (health <= 140 && transform.localScale == new Vector3(4f, 4f, 4f))
        {
            transform.localScale = new Vector3(3f, 3f, 3f);
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
        else if (health <= 0 && size == 1f)
        {
            // If health drops to 0 and size is 1, defeat the splitter
            base.Defeat();
        }
    }
    private void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            // Call TakeDamage() each time the Splitter is hit by a player projectile
            TakeDamage(damagePerHit);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(20);
            }
            // Optionally, you can destroy the bullet after dealing damage
            Explode();
        }
    }
}
