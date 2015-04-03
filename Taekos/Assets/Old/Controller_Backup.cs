//For ground checking, change radius to area

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controller_Backup : MonoBehaviour {

	public float characterHeight;
	public Text jumpText;
	public float maxHSpeed = 5f;
	private float hVelocity;
	private float hAccel = 0.4f;
	private float jumpSpeed = 10f;
	private float wallJumpSpeed = 5f;
	public bool facingRight = true;
	bool grounded = false;
	bool touchingWall = false;
	bool wallCling = false;
	private bool pressedSpace;
	public Transform groundCheck;
	public Transform wallCheck;
	float groundRadius = 0.2f;
	float wallRadius = 0.2f;
	public LayerMask whatIsGround;
	public float jumpForce = 700f;
	public float wallJumpForce = 500f;

	void Start () {
		characterHeight = 1.25f;
		hVelocity = 0f;
	}

	//Do not need to use Time.deltaTime
	void FixedUpdate () {
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		//Vector2 wallCheck1 = new Vector2 (wallCheck.position.x - 0.08f, wallCheck.position.y - (characterHeight / 2));
		//Vector2 wallCheck2 = new Vector2 (wallCheck.position.x + 0.08f, wallCheck.position.y + (characterHeight / 2));
		touchingWall = Physics2D.OverlapCircle (wallCheck.position, wallRadius, whatIsGround);
		//touchingWall = Physics2D.OverlapArea (wallCheck1, wallCheck2, whatIsGround);
		UpdateMovement ();
		//float move = Input.GetAxis ("Horizontal");
		if (touchingWall) {
			//Facing right
			if (facingRight) {
				if (hVelocity > 0) {
					hVelocity = 0;
				}
				//Do not cling to wall if grounded
				if (Input.GetKey (KeyCode.RightArrow) && rigidbody2D.velocity.y <= 0 && !grounded) {
					wallCling = true;
				} else {
					wallCling = false;
				}
			}
			//Facing left
			else if (!facingRight) {
				if (hVelocity < 0) {
					hVelocity = 0;
				}
				//Do not cling to wall if grounded
				if (Input.GetKey (KeyCode.LeftArrow) && rigidbody2D.velocity.y <= 0 && !grounded) {
					wallCling = true;
				} else {
					wallCling = false;
				}
			}
		}

		//Wall jumping
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (wallCling){
				if (!grounded){
					wallCling = false;
					//rigidbody2D.AddForce (new Vector2(wallJumpForce, jumpForce));
					hVelocity = wallJumpSpeed;
					rigidbody2D.velocity = new Vector2(wallJumpSpeed, jumpSpeed);
				}
			}
			if (grounded){
				//rigidbody2D.AddForce (new Vector2(0, jumpForce));
				rigidbody2D.velocity = new Vector2(0, jumpSpeed);
			}
		}
		if (wallCling) {
			hVelocity = 0;
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, 0.0f);
			rigidbody2D.gravityScale = 0;
		}
		else {
			rigidbody2D.gravityScale = 1;
		}

		rigidbody2D.velocity = new Vector2 (hVelocity, rigidbody2D.velocity.y);
		//Debug.Log (Input.GetKey (KeyCode.LeftArrow));
		if (hVelocity > 0 && !facingRight) {
			Flip ();
		}
		else if (hVelocity < 0 && facingRight) {
			Flip ();
		}
		Debug.Log ("Grounded:" + grounded + " Wall cling: " + wallCling + " Touching wall: " + touchingWall);
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (jumpText.active) {
				jumpText.active = false;
			} else {
				jumpText.active = true;
			}
		}
	}

	void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void UpdateMovement(){
		//If holding left arrow
		if (Input.GetKey (KeyCode.LeftArrow)) {
			if (hVelocity > -maxHSpeed) {
				hVelocity -= hAccel;
			}
			if (hVelocity < -maxHSpeed) {
				hVelocity = -maxHSpeed;
			}
		}
		//If holding right arrow
		else if (Input.GetKey (KeyCode.RightArrow)) {
			if (hVelocity < maxHSpeed) {
				hVelocity += hAccel;
			}
			if (hVelocity > maxHSpeed) {
				hVelocity = maxHSpeed;
			}
		}
		//If not holding left or right
		else if (!Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKeyDown (KeyCode.LeftArrow) && !Input.GetKeyDown (KeyCode.RightArrow)) {
			//If horizontal velocity very close to 0
			if ((hVelocity < hAccel && hVelocity > 0) ||(hVelocity > -hAccel && hVelocity < 0)){
				hVelocity = 0;
			}
			//Otherwise
			else{
				//If horizontal velocity is left
				if (hVelocity < 0){
					hVelocity += hAccel;
				}
				//If horizontal velocity is right
				else if (hVelocity > 0){
					hVelocity -= hAccel;
				}
			}
		}
	}
}
