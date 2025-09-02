using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraFireballAttack : MonoBehaviour
{
    [Range(0f, 1f)]
    public float fireballSoundVolume = 1f;

    public GameObject fireballPrefab;
    public float fireballCooldown = 2.0f; // Time between fireball attacks

    private Transform playerTransform;
    private float timeSinceLastFireball;

    public AudioClip fireballSound; // Sound when fireball is fired
    private AudioSource audioSource;


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        timeSinceLastFireball = fireballCooldown;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = fireballSoundVolume;
    }
    void Update()
    {
        timeSinceLastFireball += Time.deltaTime;

        if (timeSinceLastFireball >= fireballCooldown)
        {
            FireFireball();
            timeSinceLastFireball = 0;
        }
    }
    // You can use a timer or an event to call this method when you want to fire a fireball
    void FireFireball()
    {
        // Check if the player GameObject is not destroyed
        if (playerTransform != null)
        {
            // Calculate the direction vector from the Hydra head to the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;

            // Instantiate the fireball prefab at the Hydra head's position
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

            // Assign the direction vector to the fireball's script
            FireballController fireballController = fireball.GetComponent<FireballController>();
            fireballController.direction = direction;

            if (fireballSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(fireballSound);
            }
        }
    }
}