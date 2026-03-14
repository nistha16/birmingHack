using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gm;

    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = Vector2.left * (speed + gm.speedMult);
    }
}
