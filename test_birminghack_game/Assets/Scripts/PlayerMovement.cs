using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public GameManager gm;

    [Header("Player Settings")]
    public float jump = 10f;
    public float minSpeed = 30f;
    public float maxSpeed = 25f;
    public float backflipSpeed = 360f;
    public float boostForce = 15f;

    [Header("Sprite References")]
    public Sprite skateSprite;
    public Sprite jumpSprite;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private InputAction jumpAction;

    private bool isHoldingJump;
    private bool isFlipping;

    private SpriteRenderer sr;

    private float rotationAccumulated = 0f;
    private float lastX = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.Enable();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * Mathf.Clamp(rb.linearVelocity.magnitude, minSpeed, maxSpeed);
        
        // gentle push forward, gravity handles downhill
        // if (rb.linearVelocity.x < maxSpeed)
        // {
        //     rb.AddForce(Vector2.right * moveSpeed, ForceMode2D.Force);
        // }

        // never go backward
        if (rb.linearVelocity.x < 0f)
        {
            rb.linearVelocity = new Vector2(0,0);
            rb.AddForce(Vector2.right * minSpeed, ForceMode2D.Force);
        }
    }

    void Update()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rotationAccumulated = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            isHoldingJump = true;
        }
        
        // backflip while holding space in air
        if (jumpAction.IsPressed() && !isGrounded && isHoldingJump)
        {
            isFlipping = true;

            float rotationThisFrame = backflipSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationThisFrame);

            rotationAccumulated += rotationThisFrame;
        }

        // release space — stop flipping
        if (jumpAction.WasReleasedThisFrame())
        {
            isHoldingJump = false;
        }

        

        // land — reset rotation
        if (isGrounded && isFlipping)
        {
            transform.rotation = Quaternion.identity;

            // check for completed flip
            if (rotationAccumulated >= 360f)
            {
                rb.AddForce(Vector2.right * boostForce, ForceMode2D.Impulse);
            }

            rotationAccumulated = 0f;
            isFlipping = false;
        }

        if (!isGrounded)
        {
            sr.sprite = jumpSprite;
        }
        else
        {
            sr.sprite = skateSprite;
        }

        float distanceMoved = transform.position.x - lastX;

        if (distanceMoved > 0)
        {
            gm.score += distanceMoved;
        }

        lastX = transform.position.x;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene(0);
        }

        if (other.gameObject.CompareTag("QuestionCloud"))
        {
            StoryManager sm = FindFirstObjectByType<StoryManager>();
            if (sm != null) sm.OnCloudTriggered();
        }
    }

    void OnDisable()
    {
        jumpAction.Disable();
    }
}
