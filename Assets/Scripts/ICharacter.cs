using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICharacter{

    void movePlayer(Vector3 velocity, bool standingOnPlatform);
    void movePlayer(Vector3 velocity,Vector2 input, bool standingOnPlatform);
    void jump();
    void attack();
    void swap();
    void dash();
    CollisionInfo getCollisions();
    Vector2 getInput();
}

public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;
    public bool climbingSlope;
    public float slopeAngle, slopeAngleOld;
    public bool descendingSlope;
    public Vector3 moveAmountOld;
    public int faceDir;
    public bool fallingThroughPlatform;
    public bool slidingDownSlope;
    public bool dash;

    public void reset()
    {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;
        slopeAngleOld = slopeAngle;
        slopeAngle = 0;
        slidingDownSlope = false;
    }
}