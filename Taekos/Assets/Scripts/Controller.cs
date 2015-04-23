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
    public delegate void LifebarHandler(int health, int lives, int effect);
    public static event LifebarHandler UpdateLifebar;
    public Transform top;
    public Transform front;
    public Transform back;
	public Transform groundCheck;
	public Transform wallCheck;
	public Transform backWallCheck;
    public Transform bananaThrowPos;
    public GameObject banana;
	public GameObject jumpSound;
	public GameObject hurtSound;
	public GameObject peckBoxPrefab;
	public GameObject trail;
    public AudioSource checkpointSound;
    public AudioSource peckHitSound;
	public LayerMask whatIsGround;
	public float characterHeight;
	public float jumpHeightCoefficient = 0.3f;
    private GameObject touchingWallObject;
    private GameObject backTouchingWallObject;
    private GameObject groundedObject;
    private GameObject peckBox;
    private float bananaThrowSpeed = 15f;
    private float maxHSpeed = 11f;
    private float normalGravity = 2f;                 //1.5f
    private float fallingGravity = 1f;
	private float glideSpeed;
	private float hVelocity;
	private float hAccel = 1f;
    private float startingGlideSpeed = -1.5f;			//-1.5f
    private float descendAcceleration = 0.05f;			//0.02f, 0.013f
	private float startingJumpSpeed = 25f;              //20f
	private float jumpSpeed;							//15f, 22.5f
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
	private float killSpin = 5f;
	private Collider2D[] colliders;
	private CircleCollider2D circleCollider;
    private int bananaCount;
    private int peckFrames = 3;
	private int maxPlayerLife = 4;
	private int playerLife;
	private int facing = 1;
	private int invincibleFrames = 120;
    private int checkpointIndex;
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

    bool HoldingLeft()
    {
        return (Input.GetAxisRaw("Horizontal") == -1);
    }

    bool HoldingRight()
    {
        return (Input.GetAxisRaw("Horizontal") == 1);
    }

    bool PressJump()
    {
        return (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space));
    }

    bool HoldJump()
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space));
    }

    bool PressPeck()
    {
        return (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Q));
    }

    bool PressBanana()
    {
        return (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.E));
    }

    void Awake()
    {
        bananaCount = 0;
        resurrectPos = transform.position;
        peckingLocalPos = new Vector3(0.35f, 0.8f, 0f);
        checkpointIndex = 0;
    }

    void OnEnable()
    {
        HitBox.takeDamage += HurtPlayer;
        HitBox.killPlayer += KillPlayer;
        HitBox.getCheckpoint += UpdateCheckpoint;
        CollectItems.addHealth += AddHealth;
        Banana.destroyBanana += AddBanana;
    }

    void OnDisable()
    {
        HitBox.takeDamage -= HurtPlayer;
        HitBox.killPlayer -= KillPlayer;
        HitBox.getCheckpoint -= UpdateCheckpoint;
        CollectItems.addHealth -= AddHealth;
        Banana.destroyBanana -= AddBanana;
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
        UpdateLifebar(playerLife, getCurrentLives(), 0);
	}

	//Do not need to use Time.deltaTime
	void FixedUpdate () {
		if (rigidbody2D.velocity.y < maxFallSpeed) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, maxFallSpeed);
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
				if (control && HoldingRight() && rigidbody2D.velocity.y <= minWallClingSpeed && !grounded) {
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
				if (control && HoldingLeft() && rigidbody2D.velocity.y <= minWallClingSpeed && !grounded) {
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
		if (control && HoldJump() && !grounded && !wallCling && rigidbody2D.velocity.y <= 0) {
			glideSpeed -= descendAcceleration;
			if (glideSpeed < maxFallSpeed){
				glideSpeed = maxFallSpeed;
			}
            gliding = true;	//Necessary?
            StopPeck();
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, glideSpeed);
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

        if (grounded)
        {
            if (groundedObject.rigidbody2D != null)
            {
                Vector3 newPos = transform.position;
                newPos.x += groundedObject.rigidbody2D.velocity.x * Time.deltaTime;
                newPos.y += Mathf.Min(groundedObject.rigidbody2D.velocity.y * Time.deltaTime, 0f);
                transform.position = newPos;
            }
        }

        if (backTouchingWall)
        {
            if (backTouchingWallObject.rigidbody2D != null)
            {
                Vector3 newPos = transform.position;
                if (wallCling || (backTouchingWallObject.rigidbody2D.velocity.x > 0 && transform.position.x > backTouchingWallObject.transform.position.x) ||
                    (backTouchingWallObject.rigidbody2D.velocity.x < 0 && transform.position.x < backTouchingWallObject.transform.position.x))
                    newPos.x += backTouchingWallObject.rigidbody2D.velocity.x * Time.deltaTime;
                //newPos.y += backTouchingWallObject.rigidbody2D.velocity.y * Time.deltaTime;
                transform.position = newPos;
            }
        }

        if (touchingWall)
        {
            if (touchingWallObject.rigidbody2D != null)
            {
                Vector3 newPos = transform.position;
                if (wallCling || (touchingWallObject.rigidbody2D.velocity.x > 0 && transform.position.x > touchingWallObject.transform.position.x) ||
                    (touchingWallObject.rigidbody2D.velocity.x < 0 && transform.position.x < touchingWallObject.transform.position.x))
                    newPos.x += touchingWallObject.rigidbody2D.velocity.x * Time.deltaTime;
                if (wallCling)
                    newPos.y += touchingWallObject.rigidbody2D.velocity.y * Time.deltaTime;
                transform.position = newPos;
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
				rigidbody2D.gravityScale = wallClingDescend;
			}
		}
		else if (!isDead){
			rigidbody2D.gravityScale = normalGravity;
		}

		rigidbody2D.velocity = new Vector2 (hVelocity, rigidbody2D.velocity.y);
		if (hVelocity >= 0 && facing == -1 && HoldingRight() && control) {
			Flip ();
		}
		else if (hVelocity <= 0 && facing == 1 && HoldingLeft() && control) {
			Flip ();
		}
	}

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
            projectile.rigidbody2D.velocity = new Vector2(facing * bananaThrowSpeed + hVelocity, 0f);
            projectile.transform.localScale = new Vector3(facing, 1f, 1f);
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
                    if (touchingWallObject.rigidbody2D != null)
                    {
                        wallVelocity = Mathf.Abs(touchingWallObject.rigidbody2D.velocity.x);
                    }
                    hVelocity = (wallJumpSpeed + wallVelocity) * -facing;
                    rigidbody2D.velocity = new Vector2(hVelocity, jumpSpeed);
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
        anim.SetBool("gliding", gliding);
		if (isDead) {
			//Kill spin
		}
	}

    public int getFacing()
    {
        return facing;
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

    void UpdateCheckpoint(Vector3 checkpointPos, int index)
    {
        if (checkpointIndex < index)
        {
            checkpointIndex = index;
            checkpointSound.Play();
            resurrectPos = checkpointPos;
        }
    }

    void AddBanana(int add)
    {
        bananaCount += add;
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
        UpdateLifebar(playerLife, getCurrentLives(), 2);
    }

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
            hurtSound.audio.Play();
        }
        UpdateLifebar(playerLife, getCurrentLives(), 1);
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
        hurtSound.audio.Play();
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
        UpdateLifebar(playerLife, getCurrentLives(), 0);
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
        for (int i = 0; i < peckFrames; i++)
        {
            yield return 0;
        }
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

    void PlayPeckSound()
    {
        peckHitSound.Play();
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
