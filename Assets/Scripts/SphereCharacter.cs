using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCharacter : RayCastController { 
public LayerMask collisionMask;

private float jumpForce = 4;
private bool initJump = true;
private GameObject nextCharacter;

public float maxSlopeAngle = 50;

public CollisionInfo collisions;
private Vector2 playerInput;

public override void Start()
{
    base.Start();
    collisions.faceDir = 1;
    //start in super class raycastcontroller followed  by mine
}
public void movePlayer(Vector3 moveAmount, bool standingOnPlatform)
{
    movePlayer(moveAmount, Vector2.zero, standingOnPlatform);
}

public void movePlayer(Vector3 moveAmount, Vector2 input, bool standingOnPlatform = false)
{
    UpdateRaycastOrigins();

    collisions.reset();
    collisions.moveAmountOld = moveAmount;
    playerInput = input;
    //Debug.Log(moveAmount.x);     
    if (moveAmount.y < 0)
        descendSlope(ref moveAmount);
    if (moveAmount.x != 0)
    {
        collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
    }

    horizontalCollision(ref moveAmount);
    if (moveAmount.y != 0)
        verticalCollision(ref moveAmount);


    //transform.Translate(moveAmount);
    transform.parent.transform.Translate(moveAmount);
    //Debug.Log("Moveamount:" + moveAmount);
    //rb.AddForce(new Vector3(input, 0, 0), ForceMode.moveAmountChange);
    if (standingOnPlatform)
    {
        collisions.below = true;
    }
}

public void verticalCollision(ref Vector3 moveAmount)
{
    float directionY = Mathf.Sign(moveAmount.y);
    float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

    for (int i = 0; i < verticalRayCount; i++)
    {
        Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
        rayOrigin += Vector3.right * (verticalRaySpacing * i + moveAmount.x);
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

        if (Physics.Raycast(rayOrigin, (Vector3.up * directionY), out hit, rayLength, collisionMask))
        {
            if (hit.collider.tag.Equals("Through"))
            {
                if (directionY == 1 || hit.distance == 0)
                {
                    continue;
                }
                if (collisions.fallingThroughPlatform)
                    continue;
                if (playerInput.y == -1)
                {
                    collisions.fallingThroughPlatform = true;
                    Invoke("resetFallingThroughPlatform", 0.10f);
                    continue;
                }
            }

            if (hit.distance == 0)
            {
                continue;
            }

            moveAmount.y = (hit.distance - skinWidth) * directionY;
            rayLength = hit.distance;

            //this.transform.parent.transform.rotation = Quaternion.Euler(0,0, angle);
            if (collisions.climbingSlope)
            {
                moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                Debug.Log("climgingslops");
            }

            collisions.above = directionY == 1;
            collisions.below = directionY == -1;
        }
    }

    if (collisions.climbingSlope)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
        Vector3 RayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector3.up * moveAmount.y;
        RaycastHit hit;

        if (Physics.Raycast(RayOrigin, (Vector3.up * directionY), out hit, rayLength, collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle != collisions.slopeAngle)
            {
                moveAmount.x = (hit.distance - skinWidth) * directionX;
                collisions.slopeAngle = slopeAngle;
            }
        }
    }
}
public void horizontalCollision(ref Vector3 moveAmount)
{
    float directionX = collisions.faceDir;
    float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

    if (Mathf.Abs(moveAmount.x) < skinWidth)
    {
        rayLength = 2 * skinWidth;
    }

    for (int i = 0; i < horizontalRayCount; i++)
    {
        Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        rayOrigin += Vector3.up * (horizontalRaySpacing * i);
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

        if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, collisionMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (i == 0 && angle <= maxSlopeAngle)
            {
                if (collisions.descendingSlope)
                {
                    collisions.descendingSlope = false;
                    moveAmount = collisions.moveAmountOld;
                }

                //Debug.Log("callign climbSlope");
                float distanceToSlopeStart = 0;
                if (angle != collisions.slopeAngleOld)
                {
                    distanceToSlopeStart = hit.distance - skinWidth;
                    moveAmount.x -= distanceToSlopeStart * directionX;
                }
                climbSlope(ref moveAmount, angle);
                moveAmount.x += distanceToSlopeStart * directionX;
            }


            if (!collisions.climbingSlope || angle > maxSlopeAngle)
            {
                moveAmount.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                }
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }
}
void climbSlope(ref Vector3 moveAmount, float angle)
{
    float maxDistance = Mathf.Abs(moveAmount.x);
    float climbmoveAmountY = Mathf.Sin(angle * Mathf.Deg2Rad) * maxDistance;
    if (moveAmount.y <= climbmoveAmountY)
    {
        moveAmount.y = climbmoveAmountY;
        moveAmount.x = Mathf.Cos(angle * Mathf.Deg2Rad) * maxDistance * Mathf.Sign(moveAmount.x);
        collisions.below = true;
        collisions.climbingSlope = true;
        collisions.slopeAngle = angle;
    }
}
void descendSlope(ref Vector3 moveAmount)
{
    float directionX = Mathf.Sign(moveAmount.x);
    //Debug.Log(directionX);
    Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
    RaycastHit hit;


    if (Physics.Raycast(raycastOrigins.bottomLeft, Vector3.down, out hit, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask))
        SlideDownMaxSlope(hit, ref moveAmount);
    if (Physics.Raycast(raycastOrigins.bottomRight, Vector3.down, out hit, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask))
        SlideDownMaxSlope(hit, ref moveAmount);

    if (!collisions.slidingDownSlope)
    {
        if (Physics.Raycast(rayOrigin, Vector3.up * Mathf.Sign(moveAmount.y), out hit, Mathf.Infinity, collisionMask))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }
}

void SlideDownMaxSlope(RaycastHit hit, ref Vector3 moveAmount)
{
    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

    if (slopeAngle > maxSlopeAngle)
    {
        moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
        collisions.slopeAngle = slopeAngle;
        collisions.slidingDownSlope = true;
    }
}

void resetFallingThroughPlatform()
{
    collisions.fallingThroughPlatform = false;
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
public Vector2 getInput()
{
    return playerInput;
}
}
