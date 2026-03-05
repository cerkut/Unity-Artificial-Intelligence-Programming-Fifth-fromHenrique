using UnityEngine;

/// <summary>
/// Allows the player to shoot a bullet toward a right-clicked point on the ground.
/// Left-click moves the player (<see cref="PlayerClickToMove"/>);
/// right-click rotates the player toward the cursor and fires one bullet.
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    /// <summary>
    /// The camera used to project the mouse ray. Falls back to <see cref="Camera.main"/> if not assigned.
    /// </summary>
    public Camera cameraRef;

    /// <summary>
    /// The transform used as the bullet spawn point and forward direction.
    /// If not assigned the player's own transform is used.
    /// </summary>
    public Transform shootPoint;

    /// <summary>The bullet prefab to instantiate.</summary>
    public GameObject bullet;

    /// <summary>Launch speed of the bullet in units per second.</summary>
    public float bulletVelocity = 30f;

    /// <summary>Layer mask controlling which surfaces accept shoot destinations.</summary>
    public LayerMask layerMask = ~0;

    void Start()
    {
        if (cameraRef == null)
            cameraRef = Camera.main;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(1) || cameraRef == null || bullet == null)
            return;

        Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            return;

        // Rotate the player to face the target on the horizontal plane.
        Vector3 direction = (hit.point - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        Transform spawnTf = shootPoint != null ? shootPoint : transform;
        GameObject newBullet = Instantiate(bullet, spawnTf.position, spawnTf.rotation);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb == null)
            rb = newBullet.AddComponent<Rigidbody>();

        rb.linearVelocity = bulletVelocity * spawnTf.forward;
    }
}
