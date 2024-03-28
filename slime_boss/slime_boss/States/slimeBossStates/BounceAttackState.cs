using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/my/myBosses/slime_boss/states/slimeBossStates/Attack Enemy State")]
    public class AttackState : EnemyState
    {
        private int jumpCounter = 0;
        private Coroutine jumpCoroutine;
        private float rotationSpeed = 5f; // Speed at which the enemy rotates to face the player
        private float jumpDuration = 1.2f; // Duration of each jump
        private float jumpHeight = 10f; // Height of each jump
        private float timeBeforeJump = .5f; // Time before the first jump
        private float timeAfterJump = .5f; // Time after the last jump
        private float originalYPosition;

        protected override void OnEnter(Enemy enemy)
        {
            enemy.Gravity();
            enemy.SnapToGround();
            enemy.EntityController.CanMove = false;
            jumpCounter = 0;
            enemy.IsInvincible = true;
            StartJumping(enemy);
            // Initialize attack-related setup
            originalYPosition = enemy.transform.position.y;
        }

        protected override void OnStep(Enemy enemy) 
        {
            // Implement attack logic here
            // Increment attackCounter after each attack
            if (enemy.IsGrounded && jumpCounter < 3)
            {
                JumpRoutine(enemy);
            }
            else if (jumpCounter >= 3)
            {
                // Transition to VulnerableState after three jumps
                enemy.StateManager.TransitionToState("VulnerableState");
            }
        }

        private void StartJumping(Enemy enemy)
        {
            if (jumpCoroutine != null)
            {
                enemy.StopCoroutine(jumpCoroutine);
            }
            jumpCoroutine = enemy.StartCoroutine(JumpRoutine(enemy));
        }

        private IEnumerator JumpRoutine(Enemy enemy)
        {
            while (jumpCounter < 3)
            {
                // Wait for a certain duration before jumping
                yield return new WaitForSeconds(timeBeforeJump);

                // Smoothly rotate towards the player
                Quaternion startRotation = enemy.transform.rotation;
                Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                float rotationProgress = 0f;
                while (rotationProgress < 1f)
                {
                    rotationProgress += Time.deltaTime * rotationSpeed;
                    enemy.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationProgress);
                    yield return null;
                }

                // Animate the jump
                // Play jump sound
                enemy.GetComponent<AudioSource>().PlayOneShot(enemy.jumpSound);
                Vector3 startPosition = enemy.transform.position;
                Vector3 endPosition = new Vector3(enemy.player.position.x, startPosition.y, enemy.player.position.z); // Adjusted to maintain the same height
                float jumpTime = 0f;

                while (jumpTime < 1f)
                {
                    jumpTime += Time.deltaTime / jumpDuration;
                    float height = Mathf.Sin(Mathf.PI * jumpTime) * jumpHeight;
                    enemy.transform.position = Vector3.Lerp(startPosition, endPosition, jumpTime) + Vector3.up * height;
                    yield return null;
                }

                // Play landing sound
                enemy.GetComponent<AudioSource>().PlayOneShot(enemy.landingSound);
                // Stop any residual movement
                if (!enemy.Rigidbody.isKinematic)
                {
                    enemy.Rigidbody.velocity = Vector3.zero;
                }
                // Reset the rotation to be aligned with the ground
                enemy.transform.rotation = Quaternion.Euler(0, enemy.transform.rotation.eulerAngles.y, 0);

                // Inside the JumpRoutine coroutine, after the jump animation:
                enemy.SpawnLandingEffect();

                jumpCounter++;

                // Wait for a certain duration after landing before the next jump
                yield return new WaitForSeconds(timeAfterJump);

                // Smoothly transition back to the original Y position
                float transitionDuration = 0.5f; // Duration of the transition in seconds
                float elapsedTime = 0f;
                startPosition = enemy.transform.position;
                endPosition = new Vector3(startPosition.x, originalYPosition, startPosition.z);

                while (elapsedTime < transitionDuration)
                {
                    elapsedTime += Time.deltaTime;
                    enemy.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
                    yield return null;
                }

                // Ensure the position is exactly at the original Y position at the end of the transition
                enemy.transform.position = endPosition;
            }

            // Transition to VulnerableState
            enemy.StateManager.TransitionToState("VulnerableState");
        }











        protected override void OnExit(Enemy enemy)
        {

            enemy.IsInvincible = false;
            if (jumpCoroutine != null)
            {
                enemy.StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }
            enemy.EntityController.CanMove = true;
            // Clean up attack-related state
        }

        public override void OnContact(Enemy enemy, Collider other) { }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}