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
    /// Keeps the node running indefinitely, effectively doing nothing.
    /// Unity Native Behavior has no SUSPENDED equivalent; <see cref="Status.Running"/>
    /// is the correct value to keep a node alive without completing or failing.
    /// </summary>
    /// <returns>Always returns <see cref="Status.Running"/>.</returns>
    protected override Status OnUpdate()
    {
        return Status.Running;
    }
}

