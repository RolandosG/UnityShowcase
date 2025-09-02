using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Enemy
{
    private PlayerController m_Pc;
    public GameObject explosionPrefab; // Death animation

    public GameObject shieldPrefab;  // Prefab of the shield
    private GameObject shield;  // Instance of the shield

    public float speed;  // Speed of the shielder's movement
    public int baseHealth;  // Base health of the shielder
    private int health;  // Current health of the shielder

    public float respawnShieldTime;  // Time to respawn the shield

    private Transform playerTransform;  // Transform of the player
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private GameObject[] snipers; // Array of all snipers
    private int currentSniperIndex = 0; // Index of the current sniper being targeted

    void Start()
    {
        health = baseHealth;  // Initialize current health with base health
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            m_Pc = playerObject.GetComponent<PlayerController>();
        }

        snipers = GameObject.FindGameObjectsWithTag("Sniper");

        if (snipers.Length > 0)
        {
            ChangeTargetToSniper();
        }
    }

    void ChangeTargetToSniper()
    {
        Debug.Log("Changing target to sniper.");

        // First, check if the current sniper is not null
        if (snipers[currentSniperIndex] != null)
        {
            float targetX = snipers[currentSniperIndex].transform.position.x;
            targetPosition = new Vector3(targetX, 1f, -50f);

            // Start moving towards target position
            StartCoroutine(MoveToTargetPosition());

            // Start the coroutine to change the sniper after 3 seconds
            StartCoroutine(ChangeSniperRoutine());
        }
        else
        {
            // Handle the case when the sniper is null.
            // Maybe switch to a different target, or refresh the snipers array to exclude destroyed ones
            snipers = GameObject.FindGameObjectsWithTag("Sniper");
            currentSniperIndex = (currentSniperIndex + 1) % snipers.Length;
            ChangeTargetToSniper();
        }
    }

    IEnumerator ChangeSniperRoutine()
    {
        yield return new WaitForSeconds(3f);
        currentSniperIndex = (currentSniperIndex + 1) % snipers.Length;
        ChangeTargetToSniper();
    }

    IEnumerator MoveToTargetPosition()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        // Spawn the shield
        SpawnShield();
    }

    void Update()
    {
        /*
        if (playerTransform != null)
        {
            // Move towards the player along the x-axis with some delay
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);

            // Limit the shielder's movement along the x-axis
            float clampedX = Mathf.Clamp(transform.position.x, -7f, 7f);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
        */
    }

    private void SpawnShield()
    {
        Debug.Log("Spawning Shield");
        // If a shield is already active, don't spawn a new one
        if (shield != null)
        {
            return;
        }

        // Add an offset to the position where the shield will be instantiated.
        // Assuming that the "forward" direction for the shielder is along the positive Z-axis.
        Vector3 shieldSpawnPosition = transform.position - transform.forward;

        // Rotate the shield 90 degrees around the Y-axis.
        Quaternion shieldRotation = Quaternion.Euler(0, 90, 0);

        // Instantiate the shield at the correct position and with the correct rotation.
        shield = Instantiate(shieldPrefab, shieldSpawnPosition, shieldRotation);

        shield.transform.parent = transform;
    }

    public void ShieldDestroyed(bool dealDamage)
    {
        if (dealDamage)
        {
            health -= 10;  // Lose 10 HP
            if (health <= 0)
            {
                Defeat();
            }
        }
        StartCoroutine(RespawnShield());
    }
    IEnumerator RespawnShield()
    {
        yield return new WaitForSeconds(respawnShieldTime);

        // If a shield is already active, don't spawn a new one
        if (shield == null)
        {
            SpawnShield();
        }
    }

    public void TakeDamage(int damage)
    {
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
