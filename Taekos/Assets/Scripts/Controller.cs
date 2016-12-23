using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Taekos's character controller,
/// Handles walking, jumping, attacking, etc.
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Controller : MonoBehaviour {
	
//	public Text jumpText;
    public delegate void ControllerEventHandler(bool followPlayer);
    public static event ControllerEventHandler FollowPlayer;                    ///< Tells the camera to follow Taekos
    public delegate int GetCurrentLivesHandler();
    public static event GetCurrentLivesHandler getCurrentLives;                 ///< Requests the current number of lives
    public delegate bool GetGameOverHandler();
    public static event GetGameOverHandler getGameOver;                         ///< Checks whether or not the game is over
    public delegate void AddLivesHandler(int add);
    public static event AddLivesHandler AddLives;                               ///< Add or subtract a life
    public delegate void LifebarHandler(int health, int lives, int effect);
    public static event LifebarHandler UpdateLifebar;                           ///< Update the lifebar
    public Transform top;               ///< Top collision detection
    public Transform front;             ///< Front collision detection
    public Transform back;              ///< Back collision detection
	public Transform groundCheck;       ///< Check if Taekos is on the ground
	public Transform wallCheck;         ///< Check if Taekos is in front of a wall
	public Transform backWallCheck;     ///< Check if a wall is behind Taekos
    public Transform bananaThrowPos;    ///< Position to generate a banana from
    public GameObject banana;           ///< Banana weapon prefab
	public GameObject jumpSound;        ///< Jumping sound effect
	public GameObject hurtSound;        ///< Hurt sound effect
	public GameObject peckBoxPrefab;    ///< Create a hitbox for hurting enemies
	public GameObject trail;            ///< Trail prefab for after getting a powerup (not used in final build)
    public AudioSource checkpointSound; ///< Sound effect for getting a checkpoint
    public AudioSource peckHitSound;    ///< Sound effect for pecking an enemy
	public LayerMask whatIsGround;      ///< Ground layer mask
	public float characterHeight;       ///< How tall Taekos is
	public float jumpHeightCoefficient = 0.3f;  ///< To increase the jump height slightly when running
    private GameObject touchingWallObject;      ///< Wall detection in front
    private GameObject backTouchingWallObject;  ///< Wall detection from behind
    private GameObject groundedObject;          ///< Ground detection
    private GameObject peckBox;                 ///< Peck Box GameObject
    private float bananaThrowSpeed = 22f;
    private float maxHSpeed = 11f;              ///< Maximum running speed
    private float normalGravity = 2f;           ///< Gravity for normal conditions      //1.5f
    private float fallingGravity = 1f;          ///< Gravity for Taekos's death animation
    private float glideSpeed;
	private float hVelocity;                    ///< Current horizontal velocity
	private float hAccel = 1f;                  ///< Run/walk acceleration
    private float startingGlideSpeed = -1.5f;			//-1.5f
    private float descendAcceleration = 0.05f;			//0.02f, 0.013f
	private float startingJumpSpeed = 25f;              //20f
	private float jumpSpeed;					///< Current jump speed		//15f, 22.5f
	private float wallJumpSpeed = 7;
    private float wallClingDescend = 4f;
	private float groundRadius = 0.2f;
	private float wallRadius = 0.3f;
	private float minWallClingSpeed = 8f;
	private float maxFallSpeed = -22f;
	private float stopJumpSpeed = 10f;
	private float hurtJumpSpeed = 10f;
	private float hurtBackSpeed = -5f;
	private float hurtTime = 0.5f;
    private float resurrectDelay = 2.5f;
	private float killSpin = 5f;                ///< Was originally going to have Taekos spin when after he died
	private Collider2D[] colliders;
	private CircleCollider2D circleCollider;
    private int bananaCount;                    ///< Number of bananas currently active, Taekos can only throw one when this is at 0
    private int peckFrames = 3;
	private int maxPlayerLife = 4;              ///< Number of hits Taekos can take
	private int playerLife;                     ///< Curren number of hits Taekos has left
	private int facing = 1;                     ///< 1 for right, -1 for left
	private int invincibleFrames = 120;
    private int checkpointIndex;
	private bool isDead;
	private bool playerKilled;                  ///< So that KillPlayer() is called only once
	private bool isHurt;
	private bool isInvincible;
	private bool control;                       ///< Allow Taekos to be controlled by the player
	private bool grounded;
	private bool gliding;
	private bool touchingWall;
	private bool backTouchingWall;
	private bool wallCling;
	private bool pecking;
	private bool kicking;
    private Vector3 resurrectPos;
    private Vector3 peckingLocalPos;
	private SpriteRenderer spriteRenderer;
	private Animator anim;

    /// <summary>
    /// Called when the player holds left
    /// </summary>
    bool HoldingLeft()
    {
        return (Input.GetAxisRaw("Horizontal") == -1);
    }

    /// <summary>
    /// Called when the player holds right
    /// </summary>
    bool HoldingRight()
    {
        return (Input.GetAxisRaw("Horizontal") == 1);
    }

    /// <summary>
    /// Called when the player presses the jump button
    /// </summary>
    bool PressJump()
    {
        return (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space));
    }

    /// <summary>
    /// Called when the player holds the jump button
    /// </summary>
    bool HoldJump()
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space));
    }

    /// <summary>
    /// Called when the player presses the peck button
    /// </summary>
    bool PressPeck()
    {
        return (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Q));
    }

    /// <summary>
    /// Called when the player presses the banana throw button
    /// </summary>
    /// <returns></returns>
    bool PressBanana()
    {
        return (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E));
    }

    /// <summary>
    /// Initialize variables
    /// </summary>
    void Awake()
    {
        bananaCount = 0;
        resurrectPos = transform.position;
        peckingLocalPos = new Vector3(0.35f, 0.8f, 0f);
        checkpointIndex = 0;
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    void OnEnable()
    {
        HitBox.takeDamage += HurtPlayer;
        HitBox.killPlayer += KillPlayer;
        HitBox.getCheckpoint += UpdateCheckpoint;
        CollectItems.addHealth += AddHealth;
        Banana.destroyBanana += AddBanana;
    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    void OnDisable()
    {
        HitBox.takeDamage -= HurtPlayer;
        HitBox.killPlayer -= KillPlayer;
        HitBox.getCheckpoint -= UpdateCheckpoint;
        CollectItems.addHealth -= AddHealth;
        Banana.destroyBanana -= AddBanana;
    }

    /// <summary>
    /// Initialize more variables
    /// </summary>
	void Start () {

		//jumpHeightCoefficient Needs to be set again?
		jumpHeightCoefficient = 0.5f;
		characterHeight = 1.25f;
		hVelocity = 0f;
		jumpSpeed = startingJumpSpeed;
		glideSpeed = startingGlideSpeed;
        playerLife = maxPlayerLife;
		isDead = false;
		playerKilled = false;
		isHurt = false;
		isInvincible = false;
		control = true;
		grounded = false;
		gliding = false;
		touchingWall = false;
		backTouchingWall = false;
		wallCling = false;
		pecking = false;
		kicking = false;
		colliders = GetComponentsInChildren<Collider2D> ();
		circleCollider = GetComponent<CircleCollider2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator> ();
        StartCoroutine(Trail());
        UpdateLifebar(playerLife, getCurrentLives(), 0);
	}

	/// <summary>
	/// Player physics
	/// </summary>
	void FixedUpdate () {
		if (GetComponent<Rigidbody2D>().velocity.y < maxFallSpeed) {
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, maxFallSpeed);
        }

		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
        if (grounded)
            groundedObject = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround).gameObject;
        touchingWall = (Physics2D.OverlapArea(top.position, front.position, whatIsGround) && !isDead);
        if (touchingWall)
        {
            touchingWallObject = Physics2D.OverlapArea(top.position, front.position, whatIsGround).gameObject;
        }
        backTouchingWall = (Physics2D.OverlapArea(top.position, back.position, whatIsGround) && !isDead);
        if (backTouchingWall)
            backTouchingWallObject = Physics2D.OverlapArea(top.position, back.position, whatIsGround).gameObject;
		UpdateMovement ();
		//If character's front is touching wall
		if (touchingWall) {
			//Facing right
			if (facing == 1) {
				if (hVelocity > 0) {
					hVelocity = 0;
				}
				//Do not cling to wall if grounded
				if (control && HoldingRight() && GetComponent<Rigidbody2D>().velocity.y <= minWallClingSpeed && !grounded) {
					wallCling = true;
                    StopPeck();
				} else {
					wallCling = false;
				}
			}
			//Facing left
			else if (facing == -1) {
				if (hVelocity < 0) {
					hVelocity = 0;
				}
				//Do not cling to wall if grounded
				if (control && HoldingLeft() && GetComponent<Rigidbody2D>().velocity.y <= minWallClingSpeed && !grounded) {
					wallCling = true;
                    StopPeck();
				} else {
					wallCling = false;
				}
			}
		}
		else{
			wallCling = false;
		}
		//If character's back is touching wall
		if (backTouchingWall) {
			//Facing right
			if (facing == 1){
				if (hVelocity < 0){
					hVelocity = 0;
				}
			}
			//Facing left
			else if (facing == -1){
				if (hVelocity > 0){
					hVelocity = 0;
				}
			}
		}

		//Gliding
		if (control && HoldJump() && !grounded && !wallCling && GetComponent<Rigidbody2D>().velocity.y <= 0) {
			glideSpeed -= descendAcceleration;
			if (glideSpeed < maxFallSpeed){
				glideSpeed = maxFallSpeed;
			}
            gliding = true;	//Necessary?
            StopPeck();
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, glideSpeed);
		}
		//Not gliding
		else {
			if (gliding){
				gliding = false;
			}
		}

		//Adjust falling speed
		if (!grounded && !gliding && !wallCling && !(control && PressJump()) && !HoldJump()){
			float newVelocityY = stopJumpSpeed + Mathf.Abs(hVelocity * jumpHeightCoefficient);
		    if (GetComponent<Rigidbody2D>().velocity.y > newVelocityY){
				GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, newVelocityY);
			}
		}

		//Reset gliding speed when touching the ground or clinging to a wall
		if (grounded || wallCling) {
			if (glideSpeed != startingGlideSpeed){
				glideSpeed = startingGlideSpeed;
			}
		}

        if (grounded)
        {
            if (groundedObject.GetComponent<Rigidbody2D>() != null)
            {
                Vector3 newPos = transform.position;
                newPos.x += groundedObject.GetComponent<Rigidbody2D>().velocity.x * Time.deltaTime;
                newPos.y += Mathf.Min(groundedObject.GetComponent<Rigidbody2D>().velocity.y * Time.deltaTime, 0f);
                transform.position = newPos;
            }
        }

        if (backTouchingWall)
        {
            if (backTouchingWallObject.GetComponent<Rigidbody2D>() != null)
            {
                Vector3 newPos = transform.position;
                if (wallCling || (backTouchingWallObject.GetComponent<Rigidbody2D>().velocity.x > 0 && transform.position.x > backTouchingWallObject.transform.position.x) ||
                    (backTouchingWallObject.GetComponent<Rigidbody2D>().velocity.x < 0 && transform.position.x < backTouchingWallObject.transform.position.x))
                    newPos.x += backTouchingWallObject.GetComponent<Rigidbody2D>().velocity.x * Time.deltaTime;
                //newPos.y += backTouchingWallObject.rigidbody2D.velocity.y * Time.deltaTime;
                transform.position = newPos;
            }
        }

        if (touchingWall)
        {
            if (touchingWallObject.GetComponent<Rigidbody2D>() != null)
            {
                Vector3 newPos = transform.position;
                if (wallCling || (touchingWallObject.GetComponent<Rigidbody2D>().velocity.x > 0 && transform.position.x > touchingWallObject.transform.position.x) ||
                    (touchingWallObject.GetComponent<Rigidbody2D>().velocity.x < 0 && transform.position.x < touchingWallObject.transform.position.x))
                    newPos.x += touchingWallObject.GetComponent<Rigidbody2D>().velocity.x * Time.deltaTime;
                if (wallCling)
                    newPos.y += touchingWallObject.GetComponent<Rigidbody2D>().velocity.y * Time.deltaTime;
                transform.position = newPos;
            }
        }

		//Cling to wall
		if (wallCling && !isDead) {
			hVelocity = 0;
			float vVelocity = GetComponent<Rigidbody2D>().velocity.y;
			if (vVelocity < 0){
				vVelocity = 0;
			}
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, vVelocity);
			if (vVelocity == 0){
				GetComponent<Rigidbody2D>().gravityScale = wallClingDescend;
			}
		}
		else if (!isDead){
			GetComponent<Rigidbody2D>().gravityScale = normalGravity;
		}

		GetComponent<Rigidbody2D>().velocity = new Vector2 (hVelocity, GetComponent<Rigidbody2D>().velocity.y);
		if (hVelocity >= 0 && facing == -1 && HoldingRight() && control) {
			Flip ();
		}
		else if (hVelocity <= 0 && facing == 1 && HoldingLeft() && control) {
			Flip ();
		}
	}

    /// <summary>
    /// Throwing a banana and updating the animator
    /// </summary>
	void Update(){
        //Debug.Log(bananaCount);
		if (control && Input.GetKeyDown (KeyCode.K)) {
			//HurtPlayer (4);
            KillPlayer();
		}
        //Peck
		if (control && PressPeck()) {
            //Cannot peck while gliding or clinging to a wall
            if (!pecking && !gliding && !wallCling)
			StartCoroutine (Peck ());	//Fix animator for transitioning to jumping animation
		}
        //Throw banana
        if (control && PressBanana() && bananaCount <= 0)
        {
            //Vector3 newPos = transform.position;
            bananaCount++;
            GameObject projectile = Instantiate(banana, bananaThrowPos.position, Quaternion.identity) as GameObject;
            if (wallCling)
            {
                projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(facing * -1f * bananaThrowSpeed + hVelocity, 0f);
                projectile.transform.localScale = new Vector3(facing * -1f, 1f, 1f);
                projectile.transform.position = new Vector3(projectile.transform.position.x + 0.5f * -facing, projectile.transform.position.y, projectile.transform.position.z);
            }
            else
            {
                projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(facing * bananaThrowSpeed + hVelocity, 0f);
                projectile.transform.localScale = new Vector3(facing, 1f, 1f);
            }
        }
		if (playerLife <= 0 && !playerKilled) {
			KillPlayer ();  //Sets playerKilled to true
		}
		//Jumping and wall jumping
		if (control && PressJump()) {
			//Wall jump
			if (wallCling){
				if (!grounded){
					wallCling = false;
                    float wallVelocity = 0f;
                    if (touchingWallObject.GetComponent<Rigidbody2D>() != null)
                    {
                        wallVelocity = Mathf.Abs(touchingWallObject.GetComponent<Rigidbody2D>().velocity.x);
                    }
                    hVelocity = (wallJumpSpeed + wallVelocity) * -facing;
                    GetComponent<Rigidbody2D>().velocity = new Vector2(hVelocity, jumpSpeed);
					Flip ();
					jumpSound.GetComponent<AudioSource>().Play();
				}
			}
			//Jump
			if (grounded){
				//Jump height increases with horizontal movement
				GetComponent<Rigidbody2D>().velocity = new Vector2(hVelocity, jumpSpeed + Mathf.Abs(hVelocity * jumpHeightCoefficient));
				jumpSound.GetComponent<AudioSource>().Play();
			}
		}
		float vSpeed = GetComponent<Rigidbody2D>().velocity.y;
		bool vSpeedIsZero = (GetComponent<Rigidbody2D>().velocity.y == 0f);
		anim.SetBool ("grounded", grounded);
		anim.SetFloat ("vSpeed", vSpeed);
		anim.SetBool("vSpeedIsZero", vSpeedIsZero);
		anim.SetBool ("isHurt", isHurt);
		anim.SetBool ("isDead", isDead);
		anim.SetBool ("isPecking", pecking);
		anim.SetFloat ("absHSpeed", Mathf.Abs (hVelocity));
		anim.SetBool ("wallCling", wallCling);
        anim.SetBool("gliding", gliding);
		if (isDead) {
			//Kill spin
		}
	}

    /// <summary>
    /// Return facing
    /// </summary>
    public int getFacing()
    {
        return facing;
    }

    /// <summary>
    /// Allows Taekos to turn around
    /// </summary>
	void Flip(){
		if (facing == 1) {
			facing = -1;
		}
		else if (facing == -1) {
			facing = 1;
		}
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    /// <summary>
    /// Called in FixedUpdate to update Taekos's velocity
    /// </summary>
	void UpdateMovement(){
		//If holding left arrow
		if (control && HoldingLeft()) {
			if (hVelocity > -maxHSpeed) {
				hVelocity -= hAccel;
			}
			if (hVelocity < -maxHSpeed) {
				hVelocity = -maxHSpeed;
			}
		}
		//If holding right arrow
		else if (control && HoldingRight()) {
			if (hVelocity < maxHSpeed) {
				hVelocity += hAccel;
			}
			if (hVelocity > maxHSpeed) {
				hVelocity = maxHSpeed;
			}
		}
		//If not holding left or right
		else if (!(control && HoldingLeft()) &&
		         !(control && HoldingRight())) {
			//If horizontal velocity very close to 0
			if ((hVelocity < hAccel && hVelocity > 0) ||(hVelocity > -hAccel && hVelocity < 0)){
				hVelocity = 0;
			}
			//Otherwise
			//Move back through the air if hurt
			else{
				//If horizontal velocity is left
				if (hVelocity < 0 && !isDead && !(isHurt && !grounded)){
					hVelocity += hAccel;
				}
				//If horizontal velocity is right
				else if (hVelocity > 0 && !isDead && !(isHurt && !grounded)){
					hVelocity -= hAccel;
				}
			}
		}
	}

    /// <summary>
    /// Update the current checkpoint index
    /// </summary>
    void UpdateCheckpoint(Vector3 checkpointPos, int index)
    {
        if (checkpointIndex < index)
        {
            checkpointIndex = index;
            checkpointSound.Play();
            resurrectPos = checkpointPos;
        }
    }

    /// <summary>
    /// Keep track of the number of bananas
    /// </summary>
    void AddBanana(int add)
    {
        bananaCount += add;
    }

    /// <summary>
    /// When picking up a mango
    /// </summary>
    /// <param name="addHealth"></param>
    void AddHealth(int addHealth)
    {
        playerLife += addHealth;
        if (playerLife > maxPlayerLife)
        {
            playerLife = maxPlayerLife;
        }
        if (playerLife < 0)
        {
            playerLife = 0;
        }
        UpdateLifebar(playerLife, getCurrentLives(), 2);
    }

    /// <summary>
    /// Called by enemies when they collide with Taekos
    /// </summary>
    public void HurtPlayer(int damage)
    {
        if (isInvincible || isDead)
        {
            return;
        }
        playerLife -= damage;
        if (playerLife < 0)
        {
            playerLife = 0;
        }
        if (playerLife > 0)
        {
            hurtSound.GetComponent<AudioSource>().Play();
        }
        UpdateLifebar(playerLife, getCurrentLives(), 1);
        if (playerLife > 0)
        {
            hVelocity = hurtBackSpeed * facing;
            Vector2 vel = new Vector2(hVelocity, hurtJumpSpeed);
            GetComponent<Rigidbody2D>().velocity = vel;
            StartCoroutine(HurtAnimation());
            StartCoroutine(Invincible());
        }
    }

    /// <summary>
    /// Called when Taekos collides with a death boundary
    /// </summary>
	void KillPlayer(){
        hurtSound.GetComponent<AudioSource>().Play();
        playerKilled = true;
        playerLife = 0;
        AddLives(-1);
        UpdateLifebar(playerLife, getCurrentLives(), 0);
        //resurrectPos = transform.position;
		isDead = true;
		control = false;
		grounded = false;
		gliding = false;
		touchingWall = false;
		backTouchingWall = false;
		wallCling = false;
		pecking = false;
		kicking = false;
        foreach (Collider2D b in colliders)
        {
            b.enabled = false;
        }
		GetComponent<Rigidbody2D>().gravityScale = fallingGravity;
		hVelocity = hurtBackSpeed * facing;
		Vector2 vel = new Vector2 (hVelocity, hurtJumpSpeed);
		GetComponent<Rigidbody2D>().velocity = vel;
        FollowPlayer(false);
        if (!getGameOver())
        {
            StartCoroutine(WaitToResurrectPlayer());
        }
	}

    /// <summary>
    /// When Taekos respawns after being killed
    /// </summary>
    void ResurrectCharacter()
    {
        playerKilled = false;
        playerLife = maxPlayerLife;
        UpdateLifebar(playerLife, getCurrentLives(), 0);
        StartCoroutine(Invincible());
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        transform.position = resurrectPos;
        isDead = false;
        control = true;
        foreach (Collider2D b in colliders)
        {
            b.enabled = true;
        }
        GetComponent<Rigidbody2D>().gravityScale = normalGravity;
        FollowPlayer(true);
    }

    /// <summary>
    /// Delay after Taekos dies to resurrect
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitToResurrectPlayer()
    {
        yield return new WaitForSeconds(resurrectDelay);
        ResurrectCharacter();
    }

    /// <summary>
    /// When Taekos collects a powerup, increase his jump height
    /// </summary>
	public IEnumerator MultiplyJumpSpeed(float scale, float duration){
		jumpSpeed *= scale;
		yield return new WaitForSeconds (duration);
		jumpSpeed = startingJumpSpeed;
	}

    /// <summary>
    /// Display a trail when Taekos has a powerup
    /// </summary>
	IEnumerator Trail(){
		while (true) {
			if (jumpSpeed != startingJumpSpeed){
				GameObject newTrail = Instantiate (trail, transform.position, transform.rotation) as GameObject;
				SpriteRenderer spr = newTrail.GetComponent<SpriteRenderer>();
				spr.sprite = spriteRenderer.sprite;

				Vector3 scale = transform.localScale;
				scale.x = facing;
				newTrail.transform.localScale = scale;
			}
			yield return 0;
		}
	}

    /// <summary>
    /// Peck attack Coroutine,
    /// Pecking stops when clinging to a wall
    /// </summary>
	IEnumerator Peck(){
        StartPeck();
        for (int i = 0; i < peckFrames; i++)
        {
            yield return 0;
        }
        StopPeck();
	}

    /// <summary>
    /// Peck attack
    /// </summary>
    void StartPeck()
    {
        pecking = true;
        peckBox = Instantiate(peckBoxPrefab) as GameObject;
        peckBox.transform.localScale = new Vector3(transform.localScale.x, peckBox.transform.localScale.y, 1f);
        peckBox.transform.parent = transform;
        peckBox.transform.localPosition = peckingLocalPos;
    }

    /// <summary>
    /// Stop pecking, destroy the hit box
    /// </summary>
    void StopPeck()
    {
        pecking = false;
        if (peckBox != null)
        {
            Destroy(peckBox);
            peckBox = null;
        }
    }

    /// <summary>
    /// Play the pecking sound
    /// </summary>
    void PlayPeckSound()
    {
        peckHitSound.Play();
    }

    /// <summary>
    /// Play the hurt animation
    /// </summary>
	IEnumerator HurtAnimation(){
		isHurt = true;
		control = false;
		yield return new WaitForSeconds (hurtTime);
		isHurt = false;
		control = true;
	}

    /// <summary>
    /// Become invincible after getting hurt or respawning
    /// </summary>
	IEnumerator Invincible(){
		isInvincible = true;
		Color full = new Color (1f, 1f, 1f, 1f);
		Color empty = new Color (1f, 1f, 1f, 0f);
		for (int i = 0; i < invincibleFrames; i++) {
			if (i%4 == 0 || i%4 == 1){
				spriteRenderer.color = full;
			}
			else{
				spriteRenderer.color = empty;
			}
			yield return 0;
		}
		spriteRenderer.color = full;
		isInvincible = false;
	}
}
