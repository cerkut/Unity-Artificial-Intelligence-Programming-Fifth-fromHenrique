using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// PlayerClickToMove moves the player to the position indicated by a left mouse click.
/// A new click immediately overrides any previous destination.
/// This script replaces the BehaviorBricks "DoneAbortableClickAndGo" behavior and works with Unity 6.
/// </summary>
public class PlayerClickToMove : MonoBehaviour
{
    /// <summary>
    /// The camera used to project the mouse ray into the scene.
    /// If not assigned, <see cref="Camera.main"/> is used.
    /// </summary>
    public Camera cameraRef;

    /// <summary>Layer mask controlling which surfaces accept click destinations.</summary>
    public LayerMask layerMask = ~0;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (cameraRef == null)
            cameraRef = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && cameraRef != null)
        {
            Ray ray = cameraRef.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                agent.SetDestination(hit.point);
        }
    }
}
