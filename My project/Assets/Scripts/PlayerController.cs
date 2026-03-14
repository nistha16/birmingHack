using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public float backflipForce = 300f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDead = false;
    private bool jumpRequested = false;
    private int backflipCount;

    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.Enable();

        // create a slippery physics material so player slides over terrain bumps
        PhysicsMaterial2D mat = new PhysicsMaterial2D("PlayerMat");
        mat.friction = 0f;
        mat.bounciness = 0f;
        rb.sharedMaterial = mat;
    }

    void Update()
    {
        if (isDead) return;

        if (jumpAction.WasPressedThisFrame())
        {
            if (isGrounded)
            {
                jumpRequested = true;
            }
            else
            {
                rb.AddTorque(backflipForce);
                backflipCount++;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // constant forward movement
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // jump
        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            jumpRequested = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            transform.rotation = Quaternion.identity;
            rb.angularVelocity = 0f;

            if (backflipCount > 0)
            {
                Debug.Log("Backflip x" + backflipCount + "!");
                backflipCount = 0;
            }
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("DEAD! Press R to restart");
    }

    void OnDisable()
    {
        jumpAction.Disable();
    }
}
