﻿using UnityEngine;
using System.Collections;

public class StateMachine : StatesInherit
{

    #region Variables
    private Player plr;
    #endregion

    // Use this for initialization
	void Start () {
        plr = GetComponent<Player>();
        ChangeState("Idle");
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (((plr.canLeft && plr.LeftPressed) || (plr.RightPressed && plr.canRight)) && (!plr.isDashing && !plr.applyGravity && !plr.isJumping))
            ChangeState("Run");
        else if ((plr.isJumping))
            ChangeState("Jump");
        else if ((!plr.isDashing))
            ChangeState("Idle");
        //else if ((plr.isHit))
        //    ChangeState("Hit");
        else
            ChangeState("Dash"); //Find better way to dash...
	}
}
