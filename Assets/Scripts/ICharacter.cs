using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICharacter{

    void movePlayer(Vector3 velocity, bool standingOnPlatform);
    void jump();
    void attack();
    void swap();
    CollisionInfo getCollisions();
}

public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;
    public bool climbingSlope;
    public float slopeAngle, slopeAngleOld;
    public bool descendingSlope;
    public Vector3 velocityOld;

    public void reset()
    {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;
        slopeAngleOld = slopeAngle;
        slopeAngle = 0;
    }
}