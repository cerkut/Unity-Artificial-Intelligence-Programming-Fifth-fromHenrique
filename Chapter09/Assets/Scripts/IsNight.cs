using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

/// <summary>
/// IsNightCondition is a Unity Behavior condition node that checks whether it is night.
/// It searches for the first GameObject tagged "MainLight" and reads the DayNightCycle
/// component to determine the current time of day.
/// If no light is found, the condition returns false.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Is Night",
    story: "It is currently night",
    category: "Chapter09",
    id: "chapter09-isnight-condition-v1")]
public partial class IsNightCondition : Condition
{
    /// <summary>Checks whether it is currently night via the DayNightCycle component.</summary>
    /// <returns>True if it is night; false otherwise.</returns>
    public override bool IsTrue()
    {
        GameObject lightGO = GameObject.FindGameObjectWithTag("MainLight");
        if (lightGO == null)
            return false;
        DayNightCycle cycle = lightGO.GetComponent<DayNightCycle>();
        return cycle != null && cycle.IsNight;
    }
}
