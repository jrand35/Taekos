using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public Sprite[] numbers;
    public SpriteRenderer number1;
    public SpriteRenderer number2;
    public SpriteRenderer need;
    public SpriteRenderer feather;
    private ParticleSystem particleSystem;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleSystem.GetComponent<Renderer>().sortingLayerName = "Doors";
    }

    void OnEnable()
    {
        GameController.UpdateDoor += UpdateDoor;
    }

    void OnDisable()
    {
        GameController.UpdateDoor -= UpdateDoor;
    }

	// Use this for initialization
	void Start () {

	}

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

    void SetActive(bool active)
    {
        if (active)
        {
            particleSystem.enableEmission = true;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            particleSystem.enableEmission = false;
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
