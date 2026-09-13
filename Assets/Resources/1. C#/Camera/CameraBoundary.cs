using UnityEngine;

/// <summary>
/// Defines a 3D box boundary that constrains the movement of a target (typically the main camera). 
/// Attach this to an empty GameObject and size the box in the Inspector. 
/// Assign this for camera/target to clamp.
/// </summary>
public class CameraBoundary : MonoBehaviour
{
    [Header("Boundary")]
    [Tooltip("Full size (width, height, depth) of the box.")]
    public Vector3 size = new Vector3(1f, 1f, 1f);
    public Vector3 centerOffset;

    [Header("Gizmo")]
    public Color gizmoColor = new Color(0f, 1f, 0.4f, 1f);
    public bool drawFilled = false;
    [Range(0f, 0.3f)]
    public float filledAlpha = 0.08f;

    /// <summary>World-space center of the box.</summary>
    [HideInInspector] public Vector3 WorldCenter => transform.position + centerOffset;

    /// <summary>World-space min corner of the box.</summary>
    [HideInInspector] public Vector3 Min => WorldCenter - size * 0.5f;

    /// <summary>World-space max corner of the box.</summary>
    [HideInInspector] public Vector3 Max => WorldCenter + size * 0.5f;

    /// <summary>
    /// Clamps a world-space point so it lies within the box.
    /// </summary>
    public Vector3 ClosestPointInBounds(Vector3 point)
    {
        Vector3 min = Min;
        Vector3 max = Max;
        point.x = Mathf.Clamp(point.x, min.x, max.x);
        point.y = Mathf.Clamp(point.y, min.y, max.y);
        point.z = Mathf.Clamp(point.z, min.z, max.z);
        return point;
    }

    /// <summary>
    /// Returns true if the given world-space point is inside the box.
    /// </summary>
    public bool Contains(Vector3 point)
    {
        Vector3 min = Min;
        Vector3 max = Max;
        return point.x >= min.x && point.x <= max.x &&
               point.y >= min.y && point.y <= max.y &&
               point.z >= min.z && point.z <= max.z;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireCube(WorldCenter, size);
        if (drawFilled)
        {
            Color fill = gizmoColor;
            fill.a = filledAlpha;
            Gizmos.color = fill;
            Gizmos.DrawCube(WorldCenter, size);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Slightly thicker-looking highlight when selected: draw corner spheres.
        Gizmos.color = gizmoColor;
        Vector3 min = Min;
        Vector3 max = Max;
        Vector3[] corners = new Vector3[]
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, max.y, max.z),
        };
        float r = Mathf.Min(size.x, size.y, size.z) * 0.02f;
        foreach (var c in corners)
        {
            Gizmos.DrawSphere(c, r);
        }
    }
}
