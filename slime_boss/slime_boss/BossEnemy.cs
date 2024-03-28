using UnityEngine;
using System.Collections;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(EnemyStatsManager))]
	[RequireComponent(typeof(EnemyStateManager))]
	[RequireComponent(typeof(WaypointManager))]
	[RequireComponent(typeof(Health))]

	[AddComponentMenu("PLAYER TWO/Platformer Project/Enemy/Enemy")]
	public class BossEnemy : Entity<Enemy>
	{
		public EnemyEvents enemyEvents;

		protected Player m_player;

		protected Collider[] m_sightOverlaps = new Collider[1024];
		protected Collider[] m_contactAttackOverlaps = new Collider[1024];

        public AudioClip jumpSound;
        public AudioClip landingSound;
        public AudioClip StunnedSound;
        public AudioClip deathSound;

        /// <summary>
		/// Particle prefab to be spawned when landing and are stunned
		/// </summary>
        public GameObject landingParticlePrefab;
        public GameObject stunnedParticlePrefab;

        /// <summary>
        /// Returns the Enemy Stats Manager instance.
        /// </summary>
        public EnemyStatsManager stats { get; protected set; }

		/// <summary>
		/// Returns the Waypoint Manager instance.
		/// </summary>
		public WaypointManager waypoints { get; protected set; }

		/// <summary>
		/// Returns the Health instance.
		/// </summary>
		public Health health { get; protected set; }

		/// <summary>
		/// Returns the instance of the Player on the Enemies sight.
		/// </summary>
		public Player player { get; protected set; }

        /// <summary>
        /// Indicates whether the enemy is currently invincible and cannot take damage.
        /// </summary>
        public bool IsInvincible { get; set; }

        /// <summary>
        /// Returns the Enemy State Manager instance.
        /// </summary>
        public EnemyStateManager StateManager { get; private set; }

        public Rigidbody Rigidbody { get; private set; }

        protected virtual void InitializeStatsManager() => stats = GetComponent<EnemyStatsManager>();
		protected virtual void InitializeWaypointsManager() => waypoints = GetComponent<WaypointManager>();
		protected virtual void InitializeHealth() => health = GetComponent<Health>();
		protected virtual void InitializeTag() => tag = GameTags.Enemy;

		/// <summary>
		/// Applies damage to this Enemy decreasing its health with proper reaction.
		/// </summary>
		/// <param name="amount">The amount of health you want to decrease.</param>
		public override void ApplyDamage(int amount, Vector3 origin)
		{
			if (!IsInvincible && !health.isEmpty && !health.recovering)
			{
				health.Damage(amount);
				enemyEvents.OnDamage?.Invoke();

				if (health.isEmpty)
				{
					controller.enabled = false;
					enemyEvents.OnDie?.Invoke();
				}
			}
		}

		/// <summary>
		/// Revives this enemy, restoring its health and reenabling its movements.
		/// </summary>
		public virtual void Revive()
		{
			if (!health.isEmpty) return;

			health.ResetHealth();
			controller.enabled = true;
			enemyEvents.OnRevive.Invoke();
		}

		public virtual void Accelerate(Vector3 direction, float acceleration, float topSpeed) =>
			Accelerate(direction, stats.current.turningDrag, acceleration, topSpeed);

		/// <summary>
		/// Smoothly sets Lateral Velocity to zero by its deceleration stats.
		/// </summary>
		public virtual void Decelerate() => Decelerate(stats.current.deceleration);

		/// <summary>
		/// Smoothly sets Lateral Velocity to zero by its friction stats.
		/// </summary>
		public virtual void Friction() => Decelerate(stats.current.friction);

		/// <summary>
		/// Applies a downward force by its gravity stats.
		/// </summary>
		public virtual void Gravity() => Gravity(stats.current.gravity);

		/// <summary>
		/// Applies a downward force when ground by its snap stats.
		/// </summary>
		public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

		/// <summary>
		/// Rotate the Enemy forward to a given direction.
		/// </summary>
		/// <param name="direction">The direction you want it to face.</param>
		public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);

        /// <summary>
        /// Returns the Entity Controller instance.
        /// </summary>
        public EntityController EntityController { get; private set; }

        public virtual void ContactAttack(Collider other)
		{
			if (!other.CompareTag(GameTags.Player)) return;
			if (!other.TryGetComponent(out Player player)) return;

			var stepping = controller.bounds.max + Vector3.down * stats.current.contactSteppingTolerance;

			if (player.isGrounded || !BoundsHelper.IsBellowPoint(controller.collider, stepping))
			{
				if (stats.current.contactPushback)
					lateralVelocity = -localForward * stats.current.contactPushBackForce;

				player.ApplyDamage(stats.current.contactDamage, transform.position);
				enemyEvents.OnPlayerContact?.Invoke();
			}
		}

        /// <summary>
        /// Indicates whether the enemy is currently grounded.
        /// </summary>
        public bool IsGrounded
        {
            get
            {
                // Adjust the ray length and layer mask as needed
                return Physics.Raycast(transform.position, Vector3.down, 0.1f, LayerMask.GetMask("Ground"));
            }
        }

        /// <summary>
        /// Handles the view sight and Player detection behaviour.
        /// </summary>
        protected virtual void HandleSight()
		{
			if (!player)
			{
				var overlaps = Physics.OverlapSphereNonAlloc(position, stats.current.spotRange, m_sightOverlaps);

				for (int i = 0; i < overlaps; i++)
				{
					if (m_sightOverlaps[i].CompareTag(GameTags.Player))
					{
						if (m_sightOverlaps[i].TryGetComponent<Player>(out var player))
						{
							this.player = player;
							enemyEvents.OnPlayerSpotted?.Invoke();
							return;
						}
					}
				}
			}
			else
			{
				var distance = Vector3.Distance(position, player.position);

				if ((player.health.current == 0) || (distance > stats.current.viewRange))
				{
					player = null;
					enemyEvents.OnPlayerScaped?.Invoke();
				}
			}
		}

        public GameObject SpawnStunnedEffect(float stunDuration)
        {
            if (stunnedParticlePrefab != null)
            {
                // Adjust the position to be above the boss's head
                float yOffset = 4.7f; // Change this value based on the size of your boss
                Vector3 effectPosition = new Vector3(transform.position.x, GetComponent<Collider>().bounds.max.y + yOffset, transform.position.z);

                var effectInstance = Instantiate(stunnedParticlePrefab, effectPosition, Quaternion.identity);

                // Start a coroutine to move the effect in a circle
                float radius = 2.0f; // Increase the radius of the circle
                float speed = 2.0f; // Speed of rotation
                StartCoroutine(MoveEffectInCircle(effectInstance.transform, radius, speed, stunDuration));

                // Return the effect instance
                return effectInstance;
            }
            else
            {
                Debug.LogWarning("Stunned particle prefab is not assigned");
                return null;
            }
        }


        /// <summary>
        /// rotates the effect around the boss head
        private IEnumerator MoveEffectInCircle(Transform effectTransform, float radius, float speed, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration && effectTransform != null)
            {
                elapsedTime += Time.deltaTime;
                float angle = elapsedTime * speed;
                Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                // Adjust the effect's position to be above the center of the spherical collider
                float yOffset = GetComponent<Collider>().bounds.center.y - transform.position.y + GetComponent<Collider>().bounds.extents.y;
                effectTransform.position = transform.position + offset + Vector3.up * yOffset;

                yield return null;
            }

            // Check if the effectTransform is still valid before destroying it
            if (effectTransform != null)
            {
                Destroy(effectTransform.gameObject);
            }
        }




        public void SpawnLandingEffect()
        {
            if (landingParticlePrefab != null)
            {
                // Add an offset to raise the effect above the ground
                float yOffset = 0.0f; // Adjust this value as needed

                Vector3 effectPosition = new Vector3(transform.position.x, GetComponent<Collider>().bounds.min.y + yOffset, transform.position.z);
                var effectInstance = Instantiate(landingParticlePrefab, effectPosition, Quaternion.identity);

                float scaleFactor = 4.0f; // Change this value to scale the effect
                effectInstance.transform.localScale *= scaleFactor;

                // Destroy the particle effect after 1 second
                Destroy(effectInstance, 2f);
            }
            else
            {
                Debug.LogWarning("Landing particle prefab is not assigned");
            }
        }

        protected override void OnUpdate()
		{
			HandleSight();
		}

		protected override void Awake()
		{

			base.Awake();
            StateManager = GetComponent<EnemyStateManager>();
            Rigidbody = GetComponent<Rigidbody>();
            EntityController = GetComponent<EntityController>();
            InitializeTag();
			InitializeStatsManager();
			InitializeWaypointsManager();
			InitializeHealth();

		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			ContactAttack(other);
		}
	}
}
