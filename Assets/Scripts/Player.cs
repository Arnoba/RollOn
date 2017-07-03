using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {

    private ICharacter currentCharacter;

    private float accelerationTimeAirborne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;
    public float maxJumpHeight = 2f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = 0.4f;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    public float timeToWallUnStick;

    private Vector3 velocity;
    private float gravity;
    private float moveSpeed = 2f;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float velocityXSmoothing;

    public Vector3 wallJumpClimb;
    public Vector3 wallJumpOff;
    public Vector3 wallJumpLeap;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;


	// Use this for initialization
	void Start () {
        currentCharacter = GetComponentInChildren<ICharacter>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}
    void Update()
    {
        calculateVelocity();
        handleWallSliding();

        currentCharacter.movePlayer(velocity * Time.deltaTime, directionalInput, false);

        if (currentCharacter.getCollisions().above || currentCharacter.getCollisions().below)
        {
            if(!(currentCharacter.getCollisions().slidingDownSlope))
                velocity.y = 0;
        }
    }
    public void setDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    public void onJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                //Debug.Log("wall Climb");
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                //Debug.Log("wall up");
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
               // Debug.Log("wall switch");
                Debug.Log(-wallDirX * wallJumpLeap.x);
                velocity.x = -wallDirX * wallJumpLeap.x;
                velocity.y = wallJumpLeap.y;
            }
        }
        if (currentCharacter.getCollisions().below)
            velocity.y = maxJumpVelocity;
    }
    public void onJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
            velocity.y = minJumpVelocity;
    }
	// Update is called once per frame

    void handleWallSliding()
    {
        wallDirX = (currentCharacter.getCollisions().left) ? -1 : 1;
        wallSliding = false;  
        if ((currentCharacter.getCollisions().left || currentCharacter.getCollisions().right) && !currentCharacter.getCollisions().below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnStick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnStick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnStick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnStick = wallStickTime;
            }
        }
    }
    void calculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (currentCharacter.getCollisions().below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        //Debug.Log(velocity.x);
        velocity.y += gravity * Time.deltaTime;
    }
}


