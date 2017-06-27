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



	// Use this for initialization
	void Start () {
        currentCharacter = GetComponentInChildren<CubeCharacter>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        int wallDirX = (currentCharacter.getCollisions().left) ? -1 : 1;

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (currentCharacter.getCollisions().below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        bool wallSliding = false;

        if(currentCharacter.getCollisions().left || currentCharacter.getCollisions().right && !currentCharacter.getCollisions().below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if(timeToWallUnStick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
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
        //Can modify for doublejump etc
        if (Input.GetButton("Jump"))
        {
            if (wallSliding)
            {
                if (wallDirX == input.x)
                {
                    Debug.Log("wall Climb");
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0)
                {
                    Debug.Log("wall up");
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    Debug.Log("wall switch");

                    velocity.x = -wallDirX * wallJumpLeap.x;
                    velocity.y = wallJumpLeap.y;
                }
            }
            if(currentCharacter.getCollisions().below)
                velocity.y = maxJumpVelocity;
        }
        if (Input.GetButtonUp("Jump"))
        {
            if(velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }
        velocity.y += gravity * Time.deltaTime;
        currentCharacter.movePlayer(velocity * Time.deltaTime, input,false);

        if (currentCharacter.getCollisions().above || currentCharacter.getCollisions().below)
        {
            velocity.y = 0;
        }
    }
}
