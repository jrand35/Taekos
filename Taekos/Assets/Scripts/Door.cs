using UnityEngine;
using System.Collections;

/// <summary>
/// The script attached to the door. The level is complete when Taekos enters through the door.
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Door : MonoBehaviour {

    public Sprite[] numbers;                ///< The sprites for the numbers on the feather counter (how many more you need)
    public SpriteRenderer number1;          ///< SpriteRenderer for the 10s digit
    public SpriteRenderer number2;          ///< SpriteRenderer for the 1s digit
    public SpriteRenderer need;             ///< "Need" text above the feather counter. Disappears when Taekos collects 8 feathers
    public SpriteRenderer feather;          ///< A feather sprite. Disappears when Taekos collects 8 feathers
    public ParticleSystem ParticleSystem;  ///< Particle system for making the door shine when it can be walked through
    private SpriteRenderer spriteRenderer;  ///< The door's SpriteRenderer

    /// <summary>
    /// Initialize variables
    /// </summary>
    void Awake()
    {
        //particleSystem = GetComponentInChildren<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ParticleSystem.GetComponent<Renderer>().sortingLayerName = "Doors";
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    void OnEnable()
    {
        GameController.UpdateDoor += UpdateDoor;
    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    void OnDisable()
    {
        GameController.UpdateDoor -= UpdateDoor;
    }

    /// <summary>
    /// Called by the GameController.UpdateDoor event, update the feather counter and possibly unlock the door
    /// </summary>
    void UpdateDoor(int numFeathers, int requiredFeathers)
    {
        if (numFeathers >= requiredFeathers)
        {
            SetActive(true);
        }
        else
        {
            SetActive(false);
        }
        int remainingFeathers = requiredFeathers - numFeathers;
        if (remainingFeathers > 0)
        {
            number1.gameObject.SetActive(true);
            number2.gameObject.SetActive(true);
            need.gameObject.SetActive(true);
            feather.gameObject.SetActive(true);
            int digit1 = remainingFeathers / 10;
            int digit2 = remainingFeathers % 10;
            Sprite sprite1 = numbers[digit1];
            Sprite sprite2 = numbers[digit2];
            number1.sprite = sprite1;
            number2.sprite = sprite2;
            if (remainingFeathers < 10)
            {
                number1.gameObject.SetActive(false);
                Vector3 newPos = number2.transform.localPosition;
                newPos.x = 0f;
                number2.transform.localPosition = newPos;
            }
            else
            {
                number1.gameObject.SetActive(true);
                Vector3 newPos = number2.transform.localPosition;
                newPos.x = 0.3f;
                number2.transform.localPosition = newPos;
            }
        }
        else
        {
            number1.gameObject.SetActive(false);
            number2.gameObject.SetActive(false);
            need.gameObject.SetActive(false);
            feather.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Unlock the door
    /// </summary>
    void SetActive(bool active)
    {
        if (active)
        {
            ParticleSystem.gameObject.SetActive(true);
            //particleSystem.emission.enabled = true;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            ParticleSystem.gameObject.SetActive(false);
            //particleSystem.emission.enabled = false;
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
