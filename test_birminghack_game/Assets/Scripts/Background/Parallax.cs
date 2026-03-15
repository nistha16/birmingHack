using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
    public float parallaxFactor;
    private float startPos, length;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        ImageFill();
    }

    private void ImageFill()
    {
        // Get the screen dimensions
        float screenHeight = 2f * Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float spriteWidth = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        float spriteHeight = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;

        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;

        // Pick the larger one to ensure coverage
        float scale = Mathf.Min(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 delta = Camera.main.transform.position - previousCameraPosition;
        // transform.position += new Vector3(delta.x * parallaxFactor, delta.y, 0);
        // previousCameraPosition = Camera.main.transform.position;

        float distance = Camera.main.transform.position.x * parallaxFactor;
        float movement = Camera.main.transform.position.x * (1 - parallaxFactor);
        transform.position = new Vector3(startPos + distance, Camera.main.transform.position.y, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
