//Created by Joshua Rand
//Possibly change ground and wall detection to areas

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controller : MonoBehaviour {
	
//	public Text jumpText;
    public delegate void ControllerEventHandler(bool followPlayer);
    public static event ControllerEventHandler FollowPlayer;
    public delegate int GetCurrentLivesHandler();
    public static event GetCurrentLivesHandler getCurrentLives;
    public delegate bool GetGameOverHandler();
    public static event GetGameOverHandler getGameOver;
    public delegate void AddLivesHandler(int add);
    public static event AddLivesHandler AddLives;
    public delegate void LifebarHandler(int health, int lives);
    public static event LifebarHandler UpdateLifebar;
	public Transform groundCheck;
	public Transform wallCheck;
	public Transform backWallCheck;
	public GameObject jumpSound;
	public GameObject hurtSound;
	public GameObject peckBoxPrefab;
	public GameObject trail;
	public LayerMask whatIsGround;
	public float characterHeight;
	public float maxHSpeed = 12f;
	public float jumpHeightCoefficient = 0.3f;
	public float peckingTime = 0.25f;
    private GameObject peckBox;
    private float normalGravity = 1.5f;
    private float fallingGravity = 1f;
	private float glideSpeed;
	private float hVelocity;
	private float hAccel = 1f;
	private float startingGlideSpeed = -1;				//-1.5f
	private float startingJumpSpeed = 20f;
	private float jumpSpeed;							//15f, 22.5f
	private float wallJumpSpeed = 7;
	private float descendAcceleration = 0.02f;			//0.02f, 0.013f
	private float groundRadius = 0.2f;
	private float wallRadius = 0.3f;
	private float minWallClingSpeed = 8f;
	private float maxFallSpeed = -22f;
	private float stopJumpSpeed = 10f;
	private float hurtJumpSpeed = 10f;
	private float hurtBackSpeed = -5f;
	private float hurtTime = 0.5f;
    private float resurrectDelay = 2.5f;
	private float killSpin = 5f;
	private Collider2D[] colliders;
	private CircleCollider2D circleCollider;
	private int maxPlayerLife = 4;
	private int playerLife;
	private int facing = 1;
	private int invincibleFrames = 120;
	private bool isDead;
	private bool playerKilled; //So that KillPlayer() is called only once
	private bool isHurt;
	private bool isInvincible;
	private bool control;
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

    void Awake()
    {
        AddLives(4);    //Temporary

        resurrectPos = transform.position;
        peckingLocalPos = new Vector3(0.35f, 0.8f, 0f);
    }

    void OnEnable()
    {
        TakeDamage.takeDamage += HurtPlayer;
        TakeDamage.getCheckpoint += UpdateCheckpoint;
        PickUpPowerups.addHealth += AddHealth;
    }

    void OnDisable()
    {
        TakeDamage.takeDamage -= HurtPlayer;
        TakeDamage.getCheckpoint -= UpdateCheckpoint;
        PickUpPowerups.addHealth -= AddHealth;
    }

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
        UpdateLifebar(playerLife, getCurrentLives());
	}

 /*   void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemies" && !isInvincible && !isDead)
        {
            EnemyController damageValue = other.gameObject.GetComponent<EnemyController>();
            hurtSound.audio.Play();
            if (damageValue != null)
            {
                HurtPlayer(damageValue.getPlayerDamageValue());
            }
        }
    }*/

	//Do not need to use Time.deltaTime
	void FixedUpdate () {
		if (rigidbody2D.velocity.y < maxFallSpeed) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, maxFallSpeed);
		}
		//float move = Input.GetAxis ("Horizontal");
		//Vector2 wallCheck1 = new Vector2 (wallCheck.position.x - 0.08f, wallCheck.position.y - (characterHeight / 2));
		//Vector2 wallCheck2 = new Vector2 (wallCheck.position.x + 0.08f, wallCheck.position.y + (characterHeight / 2));
		//touchingWall = Physics2D.OverlapArea (wallCheck1, wallCheck2, whatIsGround);
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		touchingWall = (Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsGround) && !isDead);
		backTouchingWall = (Physics2D.OverlapCircle (backWallCheck.position, wallRadius, whatIsGround) && !isDead);
		UpdateMovement ();
		//If character's front is touching wall
		if (touchingWall) {
			//Facing right
			if (facing == 1) {
				if (hVelocity > 0) {
					hVelocity = 0;
				}
				//Do not cling to wall if grounded
				if (control && Input.GetKey (KeyCode.RightArrow) && rigidbody2D.velocity.y <= minWallClingSpeed && !grounded) {
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
				if (control && Input.GetKey (KeyCode.LeftArrow) && rigidbody2D.velocity.y <= minWallClingSpeed && !grounded) {
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
		if (control && Input.GetKey (KeyCode.Space) && !grounded && !wallCling && rigidbody2D.velocity.y <= 0) {
			glideSpeed -= descendAcceleration;
			if (glideSpeed < maxFallSpeed){
				glideSpeed = maxFallSpeed;
			}
			gliding = true;	//Necessary?
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, glideSpeed);
		}
		//Not gliding
		else {
			if (gliding){
				gliding = false;
			}
		}

		//Adjust falling speed
		if (!grounded && !gliding && !wallCling && !(control && Input.GetKeyDown (KeyCode.Space)) && !Input.GetKey (KeyCode.Space)){
			float newVelocityY = stopJumpSpeed + Mathf.Abs(hVelocity * jumpHeightCoefficient);
		    if (rigidbody2D.velocity.y > newVelocityY){
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, newVelocityY);
			}
		}

		//Reset gliding speed when touching the ground or clinging to a wall
		if (grounded || wallCling) {
			if (glideSpeed != startingGlideSpeed){
				glideSpeed = startingGlideSpeed;
			}
		}

		//Cling to wall
		if (wallCling && !isDead) {
			hVelocity = 0;
			float vVelocity = rigidbody2D.velocity.y;
			if (vVelocity < 0){
				vVelocity = 0;
			}
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, vVelocity);
			if (vVelocity == 0){
				rigidbody2D.gravityScale = 0f;
			}
		}
		else if (!isDead){
			rigidbody2D.gravityScale = normalGravity;
		}

		rigidbody2D.velocity = new Vector2 (hVelocity, rigidbody2D.velocity.y);
		if (hVelocity >= 0 && facing == -1 && Input.GetKey (KeyCode.RightArrow) && control) {
			Flip ();
		}
		else if (hVelocity <= 0 && facing == 1 && Input.GetKey (KeyCode.LeftArrow) && control) {
			Flip ();
		}
		Debug.Log ("Grounded:" + grounded + " Wall cling: " + wallCling + " Touching wall: " + touchingWall + " Facing: " + facing + " Hurt: " + isHurt + " Dead: " + isDead + " Pecking: " + pecking + " hVelocity: " + hVelocity);
	}

	void Update(){
		if (control && Input.GetKeyDown (KeyCode.Z)) {
			HurtPlayer (4);
		}
		if (control && Input.GetKeyDown (KeyCode.A)) {
            if (!pecking)
			StartCoroutine (Peck ());	//Fix animator for transitioning to jumping animation
		}
		if (playerLife <= 0 && !playerKilled) {
			playerKilled = true;
			KillPlayer ();
		}
		//Jumping and wall jumping
		if (control && Input.GetKeyDown (KeyCode.Space)) {
			//Wall jump
			if (wallCling){
				if (!grounded){
					wallCling = false;
					hVelocity = wallJumpSpeed * -facing;
					rigidbody2D.velocity = new Vector2(wallJumpSpeed * -facing, jumpSpeed);
					Flip ();
					jumpSound.audio.Play();
				}
			}
			//Jump
			if (grounded){
				//Jump height increases with horizontal movement
				rigidbody2D.velocity = new Vector2(hVelocity, jumpSpeed + Mathf.Abs(hVelocity * jumpHeightCoefficient));
				jumpSound.audio.Play();
			}
		}
		float vSpeed = rigidbody2D.velocity.y;
		bool vSpeedIsZero = (rigidbody2D.velocity.y == 0f);
		anim.SetBool ("grounded", grounded);
		anim.SetFloat ("vSpeed", vSpeed);
		anim.SetBool("vSpeedIsZero", vSpeedIsZero);
		anim.SetBool ("isHurt", isHurt);
		anim.SetBool ("isDead", isDead);
		anim.SetBool ("isPecking", pecking);
		anim.SetFloat ("absHSpeed", Mathf.Abs (hVelocity));
		anim.SetBool ("wallCling", wallCling);
		if (isDead) {
			//Kill spin
		}
	}

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

	void UpdateMovement(){
		//If holding left arrow
		if (control && Input.GetKey (KeyCode.LeftArrow)) {
			if (hVelocity > -maxHSpeed) {
				hVelocity -= hAccel;
			}
			if (hVelocity < -maxHSpeed) {
				hVelocity = -maxHSpeed;
			}
		}
		//If holding right arrow
		else if (control && Input.GetKey (KeyCode.RightArrow)) {
			if (hVelocity < maxHSpeed) {
				hVelocity += hAccel;
			}
			if (hVelocity > maxHSpeed) {
				hVelocity = maxHSpeed;
			}
		}
		//If not holding left or right
		else if (!(control && Input.GetKey (KeyCode.LeftArrow)) &&
		         !(control && Input.GetKey (KeyCode.RightArrow)) &&
		         !(control && Input.GetKeyDown (KeyCode.LeftArrow)) &&
		         !(control && Input.GetKeyDown (KeyCode.RightArrow))) {
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

    void UpdateCheckpoint(Vector3 checkpointPos)
    {
        resurrectPos = checkpointPos;
    }

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
        UpdateLifebar(playerLife, getCurrentLives());
    }

    public void HurtPlayer(int damage)
    {
        if (isInvincible || isDead)
        {
            return;
        }
        hurtSound.audio.Play();
        playerLife -= damage;
        if (playerLife < 0)
        {
            playerLife = 0;
        }
        UpdateLifebar(playerLife, getCurrentLives());
        if (playerLife > 0)
        {
            hVelocity = hurtBackSpeed * facing;
            Vector2 vel = new Vector2(hVelocity, hurtJumpSpeed);
            rigidbody2D.velocity = vel;
            StartCoroutine(HurtAnimation());
            StartCoroutine(Invincible());
        }
    }

	void KillPlayer(){
        AddLives(-1);
        UpdateLifebar(playerLife, getCurrentLives());
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
		rigidbody2D.gravityScale = fallingGravity;
		hVelocity = hurtBackSpeed * facing;
		Vector2 vel = new Vector2 (hVelocity, hurtJumpSpeed);
		rigidbody2D.velocity = vel;
        FollowPlayer(false);
        if (!getGameOver())
        {
            StartCoroutine(WaitToResurrectPlayer());
        }
	}

    void ResurrectCharacter()
    {
        playerKilled = false;
        playerLife = maxPlayerLife;
        UpdateLifebar(playerLife, getCurrentLives());
        StartCoroutine(Invincible());
        rigidbody2D.velocity = new Vector2(0f, 0f);
        transform.position = resurrectPos;
        isDead = false;
        control = true;
        foreach (Collider2D b in colliders)
        {
            b.enabled = true;
        }
        rigidbody2D.gravityScale = normalGravity;
        FollowPlayer(true);
    }

    IEnumerator WaitToResurrectPlayer()
    {
        yield return new WaitForSeconds(resurrectDelay);
        ResurrectCharacter();
    }

	public IEnumerator MultiplyJumpSpeed(float scale, float duration){
		jumpSpeed *= scale;
		yield return new WaitForSeconds (duration);
		jumpSpeed = startingJumpSpeed;
	}

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

    //Pecking stops when clinging to a wall
	IEnumerator Peck(){
        StartPeck();
        yield return new WaitForSeconds(peckingTime);
        StopPeck();
	}

    void StartPeck()
    {
        pecking = true;
        peckBox = Instantiate(peckBoxPrefab) as GameObject;
        peckBox.transform.localScale = new Vector3(transform.localScale.x, peckBox.transform.localScale.y, 1f);
        peckBox.transform.parent = transform;
        peckBox.transform.localPosition = peckingLocalPos;
    }

    void StopPeck()
    {
        pecking = false;
        if (peckBox != null)
        {
            Destroy(peckBox);
            peckBox = null;
        }
    }

	IEnumerator HurtAnimation(){
		isHurt = true;
		control = false;
		yield return new WaitForSeconds (hurtTime);
		isHurt = false;
		control = true;
	}

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
