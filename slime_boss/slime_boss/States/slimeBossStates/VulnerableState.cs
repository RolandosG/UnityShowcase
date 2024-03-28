using UnityEngine;
using System.Collections;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/my/myBosses/slime_boss/states/slimeBossStates/Vulnerable Enemy State")]
    public class VulnerableState : EnemyState
    {

        private float stunDuration = 5f; // Duration of the stun in seconds
        private float jumpBackDuration = .5f; // Duration of the jump back
        private float jumpBackDistance = 3f; // Distance of the jump back
        private float jumpBackHeight = 8f; // Height of the jump back
        private int initialHealth; // Health of the enemy at the start of the vulnerable state
        private GameObject stunnedEffectInstance;

        protected override void OnEnter(Enemy enemy)
        {
            enemy.IsInvincible = false;
            if (!enemy.Rigidbody.isKinematic)
            {
                enemy.Rigidbody.velocity = Vector3.zero;
            }
            enemy.EntityController.CanMove = false;
            initialHealth = enemy.health.current;
            enemy.StartCoroutine(StunCoroutine(enemy));
            stunnedEffectInstance = enemy.SpawnStunnedEffect(stunDuration);

            enemy.StartCoroutine(PlayStunSound(enemy, stunDuration / 2));
        }

        private IEnumerator StunCoroutine(Enemy enemy)
        {
            float elapsedTime = 0f;
            // Store the original y position
            float originalYPosition = enemy.transform.position.y;

            while (elapsedTime < stunDuration)
            {
                // Check if the enemy's health has decreased
                if (enemy.health.current < initialHealth)
                {

                    // Check if the player is still valid
                    if (enemy.player != null)
                    {
                        // The enemy has taken damage, keep facing the player
                        Quaternion targetRotation = Quaternion.LookRotation(enemy.player.position - enemy.transform.position);
                        enemy.transform.rotation = targetRotation;
                    }

                    // Animate the jump back directly away from the player
                    Vector3 startPosition = enemy.transform.position;
                    Vector3 backwardDirection = (startPosition - enemy.player.position).normalized; // Direction away from the player
                    Vector3 endPosition = startPosition + backwardDirection * jumpBackDistance; // Position backward from the start
                    float jumpTime = 0f;

                    while (jumpTime < 1f)
                    {
                        jumpTime += Time.deltaTime / jumpBackDuration;
                        float height = Mathf.Sin(Mathf.PI * jumpTime) * jumpBackHeight;
                        Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, jumpTime) + Vector3.up * height;
                        newPosition.y = Mathf.Clamp(newPosition.y, originalYPosition, Mathf.Infinity); // Ensure Y position does not go below original
                        enemy.transform.position = newPosition;
                        yield return null;
                    }

                    enemy.SpawnLandingEffect();
                    // Transition back to the AttackState only if the enemy is still alive
                    if (!enemy.health.isEmpty)
                    {
                        enemy.StateManager.TransitionToState("AttackState");
                    }
                    else
                    {
                        enemy.transform.rotation = Quaternion.Euler(0, enemy.transform.rotation.eulerAngles.y, 0);
                        enemy.transform.position = new Vector3(enemy.transform.position.x, originalYPosition, enemy.transform.position.z);
                    }
                    // Ensure the enemy is back at the original Y position after the jump back
                    enemy.transform.position = new Vector3(enemy.transform.position.x, originalYPosition, enemy.transform.position.z);
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // The enemy has not taken damage, transition to the FollowState
            enemy.StateManager.TransitionToState("AttackState");
            
        }

        private IEnumerator PlayStunSound(Enemy enemy, float duration)
        {
            var audioSource = enemy.GetComponent<AudioSource>();
            audioSource.clip = enemy.StunnedSound;
            audioSource.loop = true;
            audioSource.Play();

            yield return new WaitForSeconds(duration);

            audioSource.loop = false;
            audioSource.Stop();
        }


        protected override void OnStep(Enemy enemy)
        {
            if (!enemy.Rigidbody.isKinematic)
            {
                enemy.Rigidbody.velocity = Vector3.zero;
            }

            // Check if the enemy's health is empty
            if (enemy.health.isEmpty)
            {
                // Play death sound before stopping the stun sound and disabling the state manager
                var audioSource = enemy.GetComponent<AudioSource>();
                audioSource.PlayOneShot(enemy.deathSound);

                // Perform necessary cleanup for death
                enemy.IsInvincible = true;
                enemy.EntityController.CanMove = false;
                if (enemy.GetComponent<Collider>() != null)
                {
                    enemy.GetComponent<Collider>().enabled = false; // Disable the collider
                }

                // Stop the stun effect
                if (stunnedEffectInstance != null)
                {
                    UnityEngine.Object.Destroy(stunnedEffectInstance);
                }

                // Stop the stun sound
                audioSource.loop = false;
                audioSource.Stop();

                // Since there's no separate DeadState, we'll just ensure that the enemy doesn't transition to any other state
                enemy.StateManager.enabled = false; // This disables the state manager component, preventing further state transitions
            }
        }


        protected override void OnExit(Enemy enemy)
        {
            enemy.IsInvincible = true; // Make the enemy invincible again when exiting the state
            enemy.EntityController.CanMove = true;

            // Stop the stun sound
            var audioSource = enemy.GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.Stop();

            if (stunnedEffectInstance != null)
            {
                UnityEngine.Object.Destroy(stunnedEffectInstance);
            }

        }

        public override void OnContact(Enemy enemy, Collider other)
        {
            // Handle contact with the player or player's attacks here if needed
        }
    }
}
