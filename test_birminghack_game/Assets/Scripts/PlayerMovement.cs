using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float jump = 10f;
    public float moveSpeed = 30f;
    public float maxSpeed = 25f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private InputAction jumpAction;

    public float backflipSpeed = 360f;
    private bool isHoldingJump;
    private bool isFlipping;

    public Sprite skateSprite;
    public Sprite jumpSprite;
    private SpriteRenderer sr;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.Enable();
    }

    void FixedUpdate()
    {
        // gentle push forward, gravity handles downhill
        if (rb.linearVelocity.x < maxSpeed)
        {
            rb.AddForce(Vector2.right * moveSpeed, ForceMode2D.Force);
        }

        // never go backward
        if (rb.linearVelocity.x < 0f)
        {
            rb.linearVelocity = new Vector2(1f, rb.linearVelocity.y);
        }
    }

    void Update()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            isHoldingJump = true;
        }
            // backflip while holding space in air
        if (jumpAction.IsPressed() && !isGrounded && isHoldingJump)
        {
            isFlipping = true;
            transform.Rotate(0f, 0f, backflipSpeed * Time.deltaTime);
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
    }

    void OnDisable()
    {
        jumpAction.Disable();
    }
}
