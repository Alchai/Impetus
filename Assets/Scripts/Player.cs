using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    #region Variables

    public const float AttacksToKill = 15.0f, MaxLevelState = 10.0f * AttacksToKill,
        RangeBlockAmount = 0.35f, LightMBlockAmount = 0.45f, HeavyMBlockAmount = 0.65f;

    private float sWinBackground, sJumpForce, sJumpHeight,
       sMoveSpeed, runSpeed = .2f, fallspeed = .2f, gravityBoost = 0f;
    public float WinLoss_Ratio = 1f;
    public float RangeAttackScale = 1.0f, MeleeLightAttackScale = 1.0f, MeleeStrongAttackScale = 1.0f;
    private int BaseAttack, BaseDefense, jumpFrames = 25;

    public bool IsBlocking, IsMovingLeft, IsMovingRight, IsAirborne, canRight = true, canLeft = true,
        applyGravity = true, canJump = false, isJumping = false, facingLeft = false, isHit = false, L_Attacking = false, H_Attacking = false;

    public int FramesForKnockback = 15;

    //CHANGE THESE VARIABLES WHEN THE BUTTONS ARE\ARENT PRESSED 
    public bool LeftPressed = false, RightPressed = false, JumpPressed = false, isDashing = false, isAttacking = false;

    private GameObject head, feet;
    private RaycastHit[] ray;

    public Client client;

    private float dashSpeed = .15f;
    private int dashFrames = 13, FramsForKnockback = 10;

    private StatesInherit SInherit;

    private GameObject smackParticles;

    #endregion

    void Start()
    {
        smackParticles = Resources.Load("Smack") as GameObject;

        client = GameObject.Find("Client").GetComponent<Client>();

        feet = transform.FindChild("feet").gameObject;
        head = transform.FindChild("head").gameObject;

        SInherit = GetComponent<StatesInherit>();
        SInherit.ChangeState("Idle");

        if (transform.eulerAngles.y == 270 || transform.eulerAngles.y == -90)
            facingLeft = true;
        else
            facingLeft = false;
    }

    void Update()
    {

        RaycastHit[] ray = Physics.RaycastAll(feet.transform.position, Vector3.down, (fallspeed + gravityBoost) * 1.5f);
        applyGravity = true;

        for (int i = 0; i < ray.Length; i++)
        {
            if (ray[i].transform.tag == "Wall")
            {
                applyGravity = false;
                canJump = true;
            }
        }
        if (applyGravity)
        {
            transform.Translate(0f, -fallspeed - gravityBoost, 0f);
            gravityBoost += .003f;
            canJump = false;
        }
        else
            gravityBoost = -.1f;

        canLeft = true;
        canRight = true;

        ray = Physics.RaycastAll(head.transform.position, Vector3.left, runSpeed * 3f);

        for (int i = 0; i < ray.Length; i++)
            if (ray[i].transform.tag == "Wall")
                canRight = false;

        ray = Physics.RaycastAll(head.transform.position, Vector3.right, runSpeed * 3f);

        for (int i = 0; i < ray.Length; i++)
            if (ray[i].transform.tag == "Wall")
                canLeft = false;

        ray = Physics.RaycastAll(feet.transform.position, Vector3.left, runSpeed * 3f);

        for (int i = 0; i < ray.Length; i++)
            if (ray[i].transform.tag == "Wall")
                canRight = false;

        ray = Physics.RaycastAll(feet.transform.position, Vector3.right, runSpeed * 3f);

        for (int i = 0; i < ray.Length; i++)
            if (ray[i].transform.tag == "Wall")
                canLeft = false;

        if (!isDashing && !isAttacking && !((canRight && RightPressed) || (canLeft && LeftPressed)) && !isJumping && SInherit.currentState.animationName != "Idle")
            SInherit.ChangeState("Idle");


        if (!isDashing && !isAttacking)
        {
            if (canLeft && LeftPressed)
            {
                if (!isJumping && !isDashing)
                    SInherit.ChangeState("Run");
                transform.Translate(new Vector3(runSpeed, 0f, 0f), Space.World);
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, new Vector3(0f, 90f, 0f), 45f);
            }
            if (canRight && RightPressed)
            {
                if (!isJumping && !isDashing)
                    SInherit.ChangeState("Run");
                transform.Translate(new Vector3(-runSpeed, 0f, 0f), Space.World);
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, new Vector3(0f, 270f, 0f), 45f);
            }
        }
    }

    public void Dash()
    {

        if (!isDashing && !isAttacking)
        {
            StopCoroutine("dash");
            StartCoroutine("dash");
        }
    }

    private IEnumerator dash()
    {
        isDashing = true;
        SInherit.ChangeState("Run");

        if (Vector3.Distance(transform.eulerAngles, new Vector3(0f, 90f, 0f)) < 150f)
            facingLeft = false;
        else
            facingLeft = true;

        if ((facingLeft && dashSpeed > 0f) || (!facingLeft && dashSpeed < 0f))
            dashSpeed *= -1f;

        for (int i = 0; i < dashFrames; i++)
        {
            if (isDashing == false || isJumping == true)
                i = dashFrames;

            transform.Translate(new Vector3(dashSpeed, 0f, 0f), Space.World);

            yield return new WaitForEndOfFrame();
        }
        isDashing = false;
    }



    public void KnockBack()
    {
        WinLoss_Ratio -= 0.1f;


        if (WinLoss_Ratio < 0.0f)
        {
            WinLoss_Ratio = 0.0f;
            client.EndGame(client.mySID);
        }

        StopCoroutine("knockBack");
        StartCoroutine("knockBack");
    }

    private IEnumerator knockBack()
    {
        print("starting local knockback for " + name);
        SInherit.ChangeState("Hit");
        GameObject go = GameObject.Instantiate(smackParticles, transform.position, Quaternion.identity) as GameObject;
        go.transform.parent = transform;
        Destroy(go, 1f);

        switch (this.client.myChar)
        {
            case 0:
                AudioManager.play("Caveman_groan2", 1.0f, this.transform.position);
                break;
            case 1:
                AudioManager.play("Cyborg_Hurt2", 1.0f, this.transform.position);
                break;
            case 2:
                AudioManager.play("Spartan_death1", 1.0f, this.transform.position);
                break;
            case 3:
                AudioManager.play("Caveman_Jump3", 1.0f, this.transform.position);
                break;
        }

        float noob = client.me.transform.position.x - client.them.transform.position.x;
        float dist = 0f;

        if (noob > 0)
            dist = -.35f;
        else
            dist = .35f;

        if (client.me == gameObject)
            dist *= -1f;

        for (int i = 0; i < FramesForKnockback; i++)
        {
            if ((dist > 0 && canLeft) || dist < 0 && canRight)
                gameObject.transform.Translate(new Vector3(dist, 0f, 0f), Space.World);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator HackAttack()
    {
        isAttacking = true;

        SInherit.ChangeState("Attack1");

        while (SInherit.currentState.animationName.Contains("Attack"))
            yield return new WaitForEndOfFrame();

        isAttacking = false;
    }

    public void Attack_LightMelee(Player Them)
    {
        StartCoroutine("HackAttack");

        //L_Attacking = true;
        ////Play Aminmation
        //SInherit.ChangeState("Attack1");
        ////Calc Attack amount
        //float AttackAmount = this.BaseAttack * this.MeleeLightAttackScale;
        ////Play an attack animation.

        ////if (Them.IsBlocking)
        ////    AttackAmount *= LightMBlockAmount;

        //Them.sWinBackground -= AttackAmount;
        //if (this.sWinBackground + AttackAmount > MaxLevelState)
        //    this.sWinBackground = MaxLevelState;
        //else if (this.sWinBackground + AttackAmount < MaxLevelState)
        //    this.sWinBackground += AttackAmount;

        ////Play sound:
        //switch (client.myChar)
        //{
        //    case 0:
        //        AudioManager.play("Swoosh_Caveman", 1.0f, this.transform.position);
        //        break;
        //    case 1:
        //        AudioManager.play("Swoosh_Future", 1.0f, this.transform.position);
        //        break;
        //    case 2:
        //        AudioManager.play("Swoosh_Spartan", 1.0f, this.transform.position);
        //        break;
        //    case 3:
        //        AudioManager.play("Swoosh_Samurai", 1.0f, this.transform.position);
        //        break;
        //}
        //L_Attacking = false;
    }

    public void Attack_HeaveyMelee(Player Them)
    {
        H_Attacking = true;
        //Play Aminmation
        SInherit.ChangeState("Attack2");
        //Calc Attack amount
        float AttackAmount = this.BaseAttack * this.MeleeStrongAttackScale;

        if (Them.IsBlocking)
            AttackAmount *= HeavyMBlockAmount;

        Them.sWinBackground -= AttackAmount;
        if (this.sWinBackground + AttackAmount > MaxLevelState)
            this.sWinBackground = MaxLevelState;
        else if (this.sWinBackground + AttackAmount < MaxLevelState)
            this.sWinBackground += AttackAmount;

        switch (this.client.myChar)
        {
            case 0:
                //AudioManager.play("Swoosh_Caveman", 1.0, this.transform.position);
                AudioManager.play("Caveman_Jump3", 1.0f, this.transform.position);
                break;
            case 1:
                AudioManager.play("Cyborg_Melee1", 1.0f, this.transform.position);
                break;
            case 2:
                AudioManager.play("Spartan_Jump1", 1.0f, this.transform.position);
                break;
            case 3:
                AudioManager.play("Swoosh_Samurai", 1.0f, this.transform.position);
                break;
        }
        H_Attacking = false;
    }

    void Attack_Ranged(Player Them)
    {
        //Play Aminmation
        SInherit.ChangeState("Ranged");
        //Calc Attack amount
        float AttackAmount = this.BaseAttack * this.RangeAttackScale;

        //if (Them.IsBlocking)
        //    AttackAmount *= RangeBlockAmount;

        Them.sWinBackground -= AttackAmount;
        if (this.sWinBackground + AttackAmount > MaxLevelState)
            this.sWinBackground = MaxLevelState;
        else if (this.sWinBackground + AttackAmount < MaxLevelState)
            this.sWinBackground += AttackAmount;
        //sound
        switch (this.client.myChar)
        {
            case 0:
                AudioManager.play("Throw_woosh1", 1.0f, this.transform.position);
                break;
            case 1:
                AudioManager.play("GS_Laser_Shoot14", 1.0f, this.transform.position);
                break;
            case 2:
                //AudioManager.play("GS_Randomizer137", 1.0f, this.transform.position);
                AudioManager.play("Throw_woosh3", 1.0f, this.transform.position);
                break;
            case 3:
                AudioManager.play("GS_Randomizer63", 1.0f, this.transform.position);
                break;
        }
    }

    public void SetBlocking(bool IsActive)
    {
        //Set Blocking Anim if true
        // else set idle
        this.IsBlocking = IsActive;
    }

    bool GetIsBlocking()
    {
        return this.IsBlocking;
    }

    public void Jump()
    {
        if (canJump && !isAttacking)
        {
            StopCoroutine("jump");
            StartCoroutine("jump");

        }
    }

    public float GetWinBackground()
    {
        return sWinBackground;
    }
    public float GetMaxLevelState()
    {
        return MaxLevelState;
    }

    private IEnumerator jump()
    {
        if (!isAttacking)
        {

            isJumping = true;
            SInherit.ChangeState("Jump");
            int counter = 0;
            float boost = .15f;

            while (counter < jumpFrames)
            {
                print("jumpframe");
                transform.Translate(0f, fallspeed * 1.4f + boost, 0f);
                boost -= .01f;
                if (boost < 0f)
                    boost = 0f;
                yield return new WaitForEndOfFrame();
                counter++;
            }
            isJumping = false;
        }
    }

    void OnGUI()
    {
        if (name.Contains("me"))
        {
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("canleft: " + canLeft);
            GUILayout.Label("canright: " + canRight);
        }
    }
}
