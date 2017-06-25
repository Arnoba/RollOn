using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCharacter : RayCastController, ICharacter
{
    public LayerMask collisionMask;

    private float jumpForce = 4;
    private bool initJump = true;
    private GameObject nextCharacter;

    private float maxClimbAngle = 120;
    private float maxDescentAngle = 100;

    public CollisionInfo collisions;


    public void movePlayer(Vector3 velocity)
    {
        collisions.reset();

        collisions.velocityOld = velocity;

        UpdateRaycastOrigins();
        if (velocity.y < 0)
            descendSlope(ref velocity);
        if (velocity.x != 0)
            horizontalCollision(ref velocity);
        if(velocity.y !=0)
            verticalCollision(ref velocity);


        transform.Translate(velocity);
        //rb.AddForce(new Vector3(input, 0, 0), ForceMode.VelocityChange);
    }

    public void verticalCollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit hit;            

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (Physics.Raycast(rayOrigin, (Vector3.up * directionY), out hit, rayLength, collisionMask))
            {

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                //this.transform.parent.transform.rotation = Quaternion.Euler(0,0, angle);
                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y/Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector3 RayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight)+Vector3.up * velocity.y;
            RaycastHit hit;

            if (Physics.Raycast(RayOrigin, (Vector3.up*directionY), out hit, rayLength, collisionMask))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                if(slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }
    public void horizontalCollision(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector3 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (horizontalRaySpacing * i);
            RaycastHit hit;

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, collisionMask))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (i == 0 && angle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    
                    //Debug.Log("callign climbSlope");
                    float distanceToSlopeStart = 0;
                    if(angle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    climbSlope(ref velocity, angle);
                    velocity.x += distanceToSlopeStart * directionX;
                }


                if (!collisions.climbingSlope || angle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.right = directionX == 1;
                    collisions.left = directionX == -1;
                }
            }
        }
    }
    void climbSlope(ref Vector3 velocity, float angle)
    {
        float maxDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * maxDistance;
        if(velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * maxDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = angle;
        }
    }
    void descendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Debug.Log(directionX);
        Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.up * Mathf.Sign(velocity.y), out hit, Mathf.Infinity, collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescentAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    public void attack()
    {
        //throw new NotImplementedException();
    }

    public void jump()
    {
        if (initJump)
        {
            //rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            initJump = false;
        }
    }

    public void swap()
    {
        //throw new NotImplementedException();
    }

    public CollisionInfo getCollisions()
    {
        return collisions;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.tag.Equals("Ground"))
        {
            Debug.Log("Collision ground");
            initJump = true;
            //doubleJump = true;
        }
    }

    /*public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void reset()
        {
            above = below = false;
            left = right = false;
        }
    }*/
}
