using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public Transform target;          // player
    public float smoothTime = 0.25f;  // how quickly camera catches up
    public Vector3 offset;            // camera offset from player

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;

        // Keep camera at a fixed depth
        targetPosition.z = -10f;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
