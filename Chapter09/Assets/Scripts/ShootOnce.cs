using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

/// <summary>
/// ShootOnceAction is a Unity Behavior action node that instantiates a bullet prefab and
/// shoots it along the forward axis of the shoot point with the specified velocity.
/// The action completes after firing a single shot.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Shoot Once",
    story: "Shoot [Bullet] from [ShootPoint] at [Velocity] units per second",
    category: "Chapter09",
    id: "chapter09-shootonce-action-v1")]
public partial class ShootOnceAction : Action
{
    /// <summary>The transform marking the bullet spawn position and direction.</summary>
    [SerializeReference] public BlackboardVariable<Transform> ShootPoint;

    /// <summary>The bullet prefab to instantiate.</summary>
    [SerializeReference] public BlackboardVariable<GameObject> Bullet;

    /// <summary>The launch speed in units per second.</summary>
    [SerializeReference] public BlackboardVariable<float> Velocity;

    /// <summary>
    /// Fires a single bullet on the first frame. Returns <see cref="Status.Failure"/> if
    /// the shoot point or bullet prefab is missing; otherwise returns <see cref="Status.Success"/>.
    /// </summary>
    protected override Status OnStart()
    {
        Transform shootPt = ShootPoint?.Value;
        GameObject bulletPrefab = Bullet?.Value;

        if (shootPt == null || bulletPrefab == null)
        {
            Debug.LogWarning("ShootOnceAction: ShootPoint or Bullet is not assigned.");
            return Status.Failure;
        }

        float vel = Velocity?.Value ?? 30f;

        GameObject newBullet = UnityEngine.Object.Instantiate(
            bulletPrefab, shootPt.position,
            shootPt.rotation * bulletPrefab.transform.rotation);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb == null)
            rb = newBullet.AddComponent<Rigidbody>();

        rb.linearVelocity = vel * shootPt.forward;

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }
}

