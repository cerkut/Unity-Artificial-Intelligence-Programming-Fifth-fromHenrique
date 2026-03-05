# Chapter 09 – Behavior Trees

This chapter demonstrates implementing **Behavior Trees** for game AI in Unity 6, using the
[Unity Native Behavior package](https://docs.unity3d.com/Packages/com.unity.behavior@1.0)
(`com.unity.behavior@1.0`).

## Project Requirements

| Requirement | Version |
|---|---|
| Unity Editor | 6000.0.x (Unity 6) |
| `com.unity.behavior` | 1.0.7 |
| `com.unity.modules.ai` | 1.0.0 |

## Scene

Open **Assets/Scenes/BehaviorTreeDemo** to see the demo.

- **Player** – **Left-click** anywhere on the floor to move. **Right-click** anywhere to face that direction and fire a bullet (uses `PlayerClickToMove.cs` + `PlayerShoot.cs`).
- **Enemy** – Follows a priority-selector behavior tree (uses `EnemyAI.cs`).
- **Directional Light** – Drives the `DayNightCycle` component that toggles between day and night.

> **NavMesh**: The baked `NavMesh.asset` is committed in `Assets/Scenes/BehaviorTreeDemo/`. Both the Player and the Enemy use a `NavMeshAgent` so click-to-move works without any extra baking step.

## How the Behavior Tree Works

The `EnemyAI` MonoBehaviour implements the following priority-selector tree directly in C#:

```
Repeat forever {
    Priority Selector {
        if IsNight              → SleepForever (do nothing)
        if distance < 7         → Shoot at player
        if distance < 20        → Chase player
        otherwise               → Wander randomly
    }
}
```

## Custom Node Scripts (Unity Behavior visual graph)

The following scripts define custom nodes for the **Unity Behavior** visual graph editor:

| Script | Type | Description |
|--------|------|-------------|
| `IsNightCondition.cs` | `Condition` | Returns true during night time |
| `SleepForeverAction.cs` | `Action` | Runs forever (idles) |
| `ShootOnceAction.cs` | `Action` | Fires one bullet |
| `ShootAction.cs` | `Action` | Fires bullets periodically (never ends) |

### Creating the Visual Behavior Graph

To recreate the enemy behavior as a **Unity Behavior graph asset** in the editor:

1. Open the **Unity Behavior** window: **Window → AI → Behavior**.
2. Create a new graph asset in `Assets/Behaviors/` and name it `EnemyTree`.
3. Build the following tree using the nodes above:
   - Add a **Repeat** decorator (loops = –1) as the root.
   - Add a **Priority Selector** as its child.
   - Add four branches to the selector, each with a guard condition and an action:
     1. Guard: **Is Night** → Action: **Sleep Forever**
     2. Guard: **Is Target Close** (distance = 7) → Action: **Shoot Repeatedly** (delay = 1 s, velocity = 30)
     3. Guard: **Is Target Close** (distance = 20) → Action: **Navigate To Location**
     4. Guard: **Always True** → Action (sub-graph): **Wander**
4. Remove the `EnemyAI` MonoBehaviour from the Enemy GameObject.
5. Add a **BehaviorGraphAgent** component to the Enemy and assign the graph asset.
6. Populate the blackboard variables: `Player`, `WanderArea`, `ShootPoint`, `Bullet`.

## Key API Changes from BehaviorBricks to Unity Native Behavior

| BehaviorBricks (old) | Unity Native Behavior (new) |
|---|---|
| `Pada1.BBCore` / `BBUnity.Actions` | `Unity.Behavior` |
| `ConditionBase` | `Condition` |
| `GOAction` / `BasePrimitiveAction` | `Action` |
| `TaskStatus.RUNNING` | `Status.Running` |
| `TaskStatus.COMPLETED` | `Status.Success` |
| `TaskStatus.FAILED` | `Status.Failure` |
| `[Condition("path")]` | `[NodeDescription(name: "...", category: "...")]` |
| `[InParam("name")]` | `[SerializeReference] public BlackboardVariable<T>` |
| `BrainComponent` (scene) | `BehaviorGraphAgent` (scene) |
| `.asset` (BehaviorBricks XML) | `.behavior` (Unity Behavior graph) |

## Unity 6 API Fixes

- `Rigidbody.velocity` → `Rigidbody.linearVelocity` (deprecated in Unity 6)
- Removed `com.unity.ads`, `com.unity.analytics`, `com.unity.purchasing` (deprecated services)
- Removed `com.unity.xr.legacyinputhelpers` (replaced by XR Interaction Toolkit)
