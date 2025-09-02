using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Enemy
{
    private PlayerController m_Pc;
    public GameObject explosionPrefab; // Death animation

    public float speed;  // Speed of the sniper enemy's movement
    public GameObject bulletPrefab;  // Bullet prefab to be shot by the sniper
    public GameObject shootAnimationPrefab;  // Animation prefab to be instantiated when the sniper shoots
    public GameObject shootAIMAnimationPrefab;  // Animation prefab to be instantiated when the sniper shoots
    public Transform bulletSpawnPoint;  // The point from where the bullet will be fired
    public float shootCooldown = 3.0f;  // Cooldown time between each 
    public float bulletSpeed = 20f;  // The speed of the bullet

    public int baseHealth;  // Base health of the sniper
    private int health;  // Current health of the sniper

    private float shootTimer;  // Timer to track when to shoot next
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    void Start()
    {
        health = baseHealth;  // Initialize current health with base health
        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            m_Pc = playerObject.GetComponent<PlayerController>();
        }
        // Set initial position and target position based on spawn location
        initialPosition = transform.position;
        float targetX = Random.Range(-7f, 7f);
        targetPosition = new Vector3(targetX, 1f, 5f);

        // Start moving towards target position
        StartCoroutine(MoveToTargetPosition());
    }

    IEnumerator MoveToTargetPosition()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        // Start shoot and charge cycle
        StartCoroutine(ShootAndChargeCycle());
    }

    IEnumerator ShootAndChargeCycle()
    {
        while (true)
        {
            // Rise up
            while (transform.position.y < 5f)
            {
                transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                yield return null;
            }

            // Play shooting animation
            PlayShootAnimation();
            yield return new WaitForSeconds(3f);  // Wait for animation to complete

            // Shoot bullet
            ShootBullet();

            // Lower back down
            while (transform.position.y > 1f)
            {
                transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
                yield return null;
            }

            // Wait for cooldown before the next shot
            yield return new WaitForSeconds(shootCooldown);
        }
    }
    IEnumerator DestroyAfterDelay(GameObject objectToDestroy, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(objectToDestroy);
    }
    void PlayShootAnimation()
    {
        // Instantiate the animation prefab at the sniper's position and rotation
        Instantiate(shootAnimationPrefab, transform.position, transform.rotation);
    }

    void ShootBullet()
    {
        // Instantiate the animation prefab at the sniper's position and rotation
        Instantiate(shootAIMAnimationPrefab, transform.position, transform.rotation);

        // First, find the player GameObject
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // Check if the player GameObject exists
        if (playerObject != null)
        {
            Transform playerTransform = playerObject.transform; // Get the transform here after the check

            // Calculate the direction vector from the sniper to the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // Instantiate and shoot bullet
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            // Assign the direction vector to the bullet's script
            SniperProjectile sniperProjectile = bullet.GetComponent<SniperProjectile>();
            sniperProjectile.direction = direction;
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Defeat();
        }
    }
    public override void Defeat()
    {
        Explode();
    }
    private void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (m_Pc != null)
        {
            m_Pc.RecieveScore();
        }

        base.Defeat();
    }
}