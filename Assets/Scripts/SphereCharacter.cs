using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCharacter : MonoBehaviour, ICharacter
{
    private Rigidbody rb;
    private float jumpForce = 5;
    private bool initJump = true;
    private bool doubleJump = true;
    private GameObject nextCharacter;
    public CollisionInfo collisions;
    SphereCollider sphereCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        nextCharacter = transform.parent.transform.GetChild(1).gameObject;
    }

    public void movePlayer(Vector3 velocity, bool standingOnPlatform)
    {
        //rb.AddForce(new Vector3(input,0,0));
    }

    public void movePlayer(Vector3 velocity,Vector2 input, bool standingOnPlatform)
    {
        //rb.AddForce(new Vector3(input,0,0));
    }

    public void attack()
    {
        throw new NotImplementedException();
    }

    public void jump()
    {
        if (initJump)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            initJump = false;
        }
        else if (doubleJump)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            doubleJump = false;
        }
    }

    public void swap()
    {
        throw new NotImplementedException();
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
            doubleJump = true;
        }
    }
    public Vector2 getInput()
    {
        return new Vector2();
    }
}
