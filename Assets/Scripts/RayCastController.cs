using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastController : MonoBehaviour {
    public const float skinWidth = 0.0015f;
    public BoxCollider collider;

    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    public RaycastOrigins raycastOrigins;

    public float horizontalRaySpacing;
    public float verticalRaySpacing;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        calculateRaySpacing();

        //nextCharacter = transform.parent.transform.GetChild(1).gameObject;
    }


    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //maybe use mid instead of min z
        raycastOrigins.bottomLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        raycastOrigins.bottomRight = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        raycastOrigins.topLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        raycastOrigins.topRight = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
    }

    public void calculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    public struct RaycastOrigins
    {
        public Vector3 topRight, topLeft;
        public Vector3 bottomRight, bottomLeft;
    }
}
