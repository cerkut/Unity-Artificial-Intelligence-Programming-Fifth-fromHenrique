using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

/// <summary>
/// ShootAction is a Unity Behavior action node that periodically fires a bullet prefab
/// along the forward axis of the shoot point. This action never completes on its own.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Shoot Repeatedly",
    story: "Shoot [Bullet] from [ShootPoint] every [Delay] seconds at [Velocity] units per second",
    category: "Chapter09",
    id: "chapter09-shoot-action-v1")]
public partial class ShootAction : Action
{
    /// <summary>The transform marking the bullet spawn position and direction.</summary>
    [SerializeReference] public BlackboardVariable<Transform> ShootPoint;

    /// <summary>The bullet prefab to instantiate.</summary>
    [SerializeReference] public BlackboardVariable<GameObject> Bullet;

    /// <summary>Seconds between shots. Defaults to 1 second.</summary>
    [SerializeReference] public BlackboardVariable<float> Delay;

    /// <summary>The launch speed in units per second.</summary>
    [SerializeReference] public BlackboardVariable<float> Velocity;

    private float elapsedTime;

    /// <summary>Resets the shot timer so the first shot fires after one full delay.</summary>
    protected override Status OnStart()
    {
        elapsedTime = 0f;
        return Status.Running;
    }

    /// <summary>
    /// Advances the timer each frame and fires a bullet when the delay has elapsed.
    /// Always returns <see cref="Status.Running"/> to keep the action active indefinitely.
    /// </summary>
    protected override Status OnUpdate()
    {
        float delay = Delay?.Value ?? 1f;
        elapsedTime += UnityEngine.Time.deltaTime;

        if (elapsedTime < delay)
            return Status.Running;

        // Reset the timer before firing so the next shot is delayed by a full
        // period regardless of how long FireBullet() takes (it's instant, but
        // this ordering keeps the interval consistent). The first shot therefore
        // fires after the initial delay, consistent with the original Shoot action.
        elapsedTime = 0f;
        FireBullet();
        return Status.Running;
    }

    private void FireBullet()
    {
        Transform shootPt = ShootPoint?.Value;
        GameObject bulletPrefab = Bullet?.Value;

        if (shootPt == null || bulletPrefab == null)
            return;

        float vel = Velocity?.Value ?? 30f;

        GameObject newBullet = UnityEngine.Object.Instantiate(
            bulletPrefab, shootPt.position,
            shootPt.rotation * bulletPrefab.transform.rotation);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb == null)
            rb = newBullet.AddComponent<Rigidbody>();

        rb.linearVelocity = vel * shootPt.forward;
    }
}
