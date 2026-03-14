using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float jump;
    private Rigidbody2D rb;
    private bool isGrounded;
    private InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.Enable();        
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.IsPressed() && isGrounded)
        {
            rb.AddForce(Vector2.up * jump);
        }
    }

    // OnCollisionEnter2D is called on collision with another collision object
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // OnCollisionEnter2D is called on leaving collision from another collision object
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
}
