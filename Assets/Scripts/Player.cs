using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {

    private ICharacter currentCharacter;

    private float accelerationTimeAirborne = 0.8f;
    private float accelerationTimeGrounded = 0.1f;
    public float jumpHeight = 2f;
    public float timeToJumpApex = 0.4f;

    private Vector3 velocity;
    private float gravity;
    private float moveSpeed = 2f;
    private float jumpVelocity;
    private float velocityXSmoothing;



	// Use this for initialization
	void Start () {
        currentCharacter = GetComponentInChildren<CubeCharacter>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}
	
	// Update is called once per frame
	void Update () {

        if(currentCharacter.getCollisions().above||currentCharacter.getCollisions().below)
        {
            velocity.y = 0;
        }

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0);

        //Can modify for doublejump etc
        if (Input.GetButton("Jump") && currentCharacter.getCollisions().below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (currentCharacter.getCollisions().below)?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        currentCharacter.movePlayer(velocity * Time.deltaTime);
	}
}
