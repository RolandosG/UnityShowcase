using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/my/myBosses/slime_boss/states/slimeBossStates/Follow Boss State")]
    public class FollowBossState : EnemyState
    {
        protected override void OnEnter(Enemy enemy) { }

        protected override void OnExit(Enemy enemy) { }

        protected override void OnStep(Enemy enemy)
        {
            enemy.Gravity();
            enemy.SnapToGround();

            var head = enemy.player.position - enemy.position;
            var upOffset = Vector3.Dot(enemy.transform.up, head);
            var direction = head - enemy.transform.up * upOffset;
            var localDirection = Quaternion.FromToRotation(enemy.transform.up, Vector3.up) * direction;
            localDirection = localDirection.normalized;

            if (enemy.player == null)
            {
                Debug.LogWarning("Player reference is null in FollowBossState");
                return;
            }

            if (enemy.StateManager == null)
            {
                Debug.LogWarning("StateManager reference is null in FollowBossState");
                return;
            }

            // Check if the player is within the attack range
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
            if (distanceToPlayer <= enemy.stats.current.attackRange)
            {
                // Stop the enemy's movement
                if (!enemy.Rigidbody.isKinematic)
                {
                    enemy.Rigidbody.velocity = Vector3.zero;
                }
                // Transition to the AttackState
                enemy.StateManager.TransitionToState("AttackState");
            }
            else
            {
                if (enemy.Rigidbody.isKinematic)
                {
                    // Move the enemy by setting its position directly
                    Vector3 newPosition = enemy.transform.position + localDirection * enemy.stats.current.followAcceleration * Time.deltaTime;
                    enemy.transform.position = newPosition;
                }
                else
                {
                    enemy.Accelerate(localDirection, enemy.stats.current.followAcceleration, enemy.stats.current.followTopSpeed);
                }
                enemy.FaceDirectionSmooth(localDirection);
            }
        }

        public override void OnContact(Enemy enemy, Collider other) { }
    }
}
