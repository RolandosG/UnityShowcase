using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamController : MonoBehaviour
{
    public Transform hydraHead;
    public float laserSpeed = 15.0f;
    public float laserResetCooldown = 4.0f;
    public float damageCooldown = 2.0f;

    private Transform playerTransform;
    private float initialXRotation = 6.0f;
    private float targetYRotation;
    private bool targetYRotationSet = false;

    private float timeSinceLastReset = 0.0f;
    private bool isLaserActive = true;
    private float timeSinceLastDamage = 0.0f;
    private LineRenderer lineRenderer;
    public AudioClip laserSound;
    private AudioSource audioSource;
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Update the timer
        timeSinceLastReset += Time.deltaTime;
        timeSinceLastDamage += Time.deltaTime;
        // Check if the cooldown has passed
        if (timeSinceLastReset >= laserResetCooldown && !isLaserActive)
        {
            // Reset the timer
            timeSinceLastReset = 0.0f;

            // Reset the initial X rotation and target Y rotation set flag
            initialXRotation = 26.0f;
            targetYRotationSet = false;
            isLaserActive = true;
        }

        if (isLaserActive)
        {
            lineRenderer.enabled = true;

            if (laserSound != null && audioSource != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(laserSound);
            }

            // Check if the playerTransform is still valid
            if (playerTransform != null)
            {
                // Calculate the direction vector from the Hydra head to the player
                Vector3 direction = (playerTransform.position - hydraHead.position).normalized;

                // Calculate the X-axis rotation based on the laserSpeed
                initialXRotation -= (laserSpeed * Time.deltaTime);
                float xRotation = Mathf.Clamp(initialXRotation, 0.0f, 23.0f);

                // Calculate the Y-axis rotation
                if (!targetYRotationSet)
                {
                    targetYRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    targetYRotationSet = true;
                }
                float yRotation = targetYRotation;

                // Set the new rotation
                transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);

                // Check if the X-axis rotation has reached zero
                if (xRotation <= 0.0f)
                {
                    isLaserActive = false;
                    timeSinceLastReset = 0.0f;
                    transform.eulerAngles = new Vector3(26.0f, yRotation, 0.0f);
                    lineRenderer.enabled = false;
                }
            }
        }
        else if (audioSource.isPlaying)
        {
            // Stop playing the sound if laser is not active
            audioSource.Stop();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && timeSinceLastDamage >= damageCooldown && isLaserActive)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.RecieveDamage();
                timeSinceLastDamage = 0.0f;
            }
        }
    }
}
