using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

/// <summary>
/// SleepForeverAction is a Unity Behavior action node that never completes.
/// It is used to keep the enemy idle (e.g., during night time) without consuming CPU.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Sleep Forever",
    story: "Sleep forever (do nothing)",
    category: "Chapter09",
    id: "chapter09-sleepforever-action-v1")]
public partial class SleepForeverAction : Action
{
    /// <summary>
    /// Suspends the node so it is not called every frame, conserving CPU.
    /// <see cref="Status.Wait"/> is the Unity Behavior equivalent of the former
    /// BehaviorBricks <c>TaskStatus.SUSPENDED</c>.
    /// </summary>
    /// <returns>Always returns <see cref="Status.Wait"/>.</returns>
    protected override Status OnUpdate()
    {
        return Status.Wait;
    }
}

