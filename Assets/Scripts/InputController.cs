using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class InputController : MonoBehaviour {
    GameObject activeMode;
    private ICharacter activeCharacter;
    Player player;

	// Use this for initialization
	void Start () {
        //activeCharacter = GetComponentInChildren<SphereCharacter>();
        player = GetComponent<Player>();
    }
	
    private void Update()
    {
        Vector2 directionalInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.setDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Jump"))
        {
            player.onJumpInputDown();
        }
        if (Input.GetButtonUp("Jump"))
        {
            player.onJumpInputUp();
        }
        if (Input.GetButtonDown("Dash"))
        {
            player.dash();
        }
    }
}
