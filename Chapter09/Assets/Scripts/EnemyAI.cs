using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// EnemyAI implements the enemy behavior tree logic directly in C#.
/// It uses a priority-selector pattern (the core of a behavior tree) evaluated each frame:
/// <list type="number">
///   <item>If it is night, the enemy sleeps (does nothing).</item>
///   <item>If the player is within <see cref="shootDistance"/>, the enemy shoots.</item>
///   <item>If the player is within <see cref="chaseDistance"/>, the enemy chases the player.</item>
///   <item>Otherwise, the enemy wanders randomly across the navigation mesh.</item>
/// </list>
/// This script replaces the BehaviorBricks component and works with Unity 6.
/// For the Unity Native Behavior visual-graph approach, see the custom node scripts
/// (IsNightCondition, ShootAction, ShootOnceAction, SleepForeverAction) and the
/// <c>EnemyTree.behavior</c> asset that must be created in the Unity Behavior editor.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Scene References")]
    /// <summary>The player GameObject that the enemy reacts to.</summary>
    public GameObject player;

    /// <summary>The floor/area GameObject used as the wander bounds.</summary>
    public GameObject wanderArea;

    /// <summary>
    /// Transform marking the bullet spawn position and forward direction.
    /// If not assigned, the script searches for a child named "shootPoint".
    /// </summary>
    public Transform shootPoint;

    /// <summary>The bullet prefab to instantiate when shooting.</summary>
    public GameObject bullet;

    [Header("Behavior Settings")]
    /// <summary>Distance at which the enemy switches from chasing to shooting.</summary>
    public float shootDistance = 7f;

    /// <summary>Distance at which the enemy starts chasing the player.</summary>
    public float chaseDistance = 20f;

    /// <summary>Seconds between shots.</summary>
    public float shootDelay = 1f;

    /// <summary>Launch speed of the bullet in units per second.</summary>
    public float bulletVelocity = 30f;

    private NavMeshAgent agent;
    private DayNightCycle dayNightCycle;
    private float shootTimer;
    private bool isWandering;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find the DayNightCycle component on the "MainLight" tagged object.
        GameObject lightGO = GameObject.FindGameObjectWithTag("MainLight");
        if (lightGO != null)
            dayNightCycle = lightGO.GetComponent<DayNightCycle>();

        // Find shoot point in children if not assigned.
        if (shootPoint == null)
            shootPoint = transform.Find("shootPoint");

        shootTimer = shootDelay;
    }

    void Update()
    {
        // Priority 1: Sleep during night time.
        if (dayNightCycle != null && dayNightCycle.IsNight)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        if (player == null)
        {
            Wander();
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Priority 2: Shoot when close enough.
        if (distToPlayer <= shootDistance)
        {
            agent.ResetPath();
            ShootAtPlayer();
            return;
        }

        // Priority 3: Chase the player when within range.
        if (distToPlayer <= chaseDistance)
        {
            isWandering = false;
            agent.SetDestination(player.transform.position);
            return;
        }

        // Priority 4: Wander randomly.
        Wander();
    }

    private void ShootAtPlayer()
    {
        if (shootPoint == null || bullet == null)
            return;

        // Rotate to face the player on the horizontal plane.
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f)
            return;

        shootTimer = shootDelay;

        GameObject newBullet = Instantiate(
            bullet, shootPoint.position,
            shootPoint.rotation * bullet.transform.rotation);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb == null)
            rb = newBullet.AddComponent<Rigidbody>();

        rb.linearVelocity = bulletVelocity * shootPoint.forward;
    }

    private void Wander()
    {
        // Pick a new destination when we arrive or haven't started yet.
        if (!isWandering || !agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 target = GetRandomNavMeshPosition();
            agent.SetDestination(target);
            isWandering = true;
        }
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 center = transform.position;
        float radius = 10f;

        if (wanderArea != null)
        {
            center = wanderArea.transform.position;
            // Use half the floor's scale as the wander extent.
            Vector3 scale = wanderArea.transform.localScale;
            radius = Mathf.Min(scale.x, scale.z) * 0.5f;
        }

        Vector3 randomPoint = center + new Vector3(
            Random.Range(-radius, radius),
            0f,
            Random.Range(-radius, radius));

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius * 2f, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }
}
