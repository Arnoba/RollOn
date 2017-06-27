using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RayCastController {
    public LayerMask passengerMask;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, ICharacter> passengerDictionary = new Dictionary<Transform, ICharacter>();

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0,2)]
    public float easeAmnount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

	// Use this for initialization
	public override void Start () {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        calculatePassangerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);

	}
    void MovePassengers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<ICharacter>());
            }
            if(passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].movePlayer(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    float Ease(float x)
    {
        float a = easeAmnount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if(Time.time< nextMoveTime)
        {
            return Vector3.zero;
        }


        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }
        //controlls anything being move by platform
    void calculatePassangerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        //Hash set very fast at adding and checking if contains


        //Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector3.right * (verticalRaySpacing * i);
                RaycastHit hit;
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
                if (Physics.Raycast(rayOrigin, (Vector3.up * directionY), out hit, rayLength, passengerMask))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        //hit.transform.Translate(new Vector3(pushX, pushY, 0));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY,0), (directionY == 1), true));
                            }
                }
            }
        }
        //Hooizontal platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector3.up * (horizontalRaySpacing * i);
                RaycastHit hit;

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

                if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, passengerMask))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        //hit.transform.Translate(new Vector3(pushX, pushY, 0));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY, 0), false, true));
                        movedPassengers.Add(hit.transform);
                    }
                }
            }
        }
        //if passenger on top of a horzontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0) { }
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector3 rayOrigin = raycastOrigins.topLeft + Vector3.right * (verticalRaySpacing * i);
                RaycastHit hit;
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
                if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayLength, passengerMask))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        //hit.transform.Translate(new Vector3(pushX, pushY, 0));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY, 0), true, false));
                        movedPassengers.Add(hit.transform);
                    }
                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocty, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocty;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    private void OnDrawGizmos()
    {
        if(localWaypoints != null)
        {
            Gizmos.color = Color.green;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointsPos = (Application.isPlaying)?globalWaypoints[i]:localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointsPos - Vector3.up * size, globalWaypointsPos + Vector3.up*size);
                Gizmos.DrawLine(globalWaypointsPos - Vector3.left * size, globalWaypointsPos + Vector3.left * size);
            }
        }
    }
}
