using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    GameObject activeMode;
    private ICharacter activeCharacter;

	// Use this for initialization
	void Start () {
        activeCharacter = GetComponentInChildren<SphereCharacter>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Movement
       // activeCharacter.movePlayer(Input.GetAxis("Horizontal"));
        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            activeCharacter.jump();
        }
	}
}
