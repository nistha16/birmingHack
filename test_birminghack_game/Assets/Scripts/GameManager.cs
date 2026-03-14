using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject spawnObject;
    public GameObject spawnPoint;

    public float obstacleRangeMin;
    public float obstacleRangeMax;

    public float speedMult;

    private float timer;
    private float obstacleInterval;

    private void GenerateInterval()
    {
        obstacleInterval = Random.Range(obstacleRangeMin, obstacleRangeMax);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateInterval();
    }

    // Update is called once per frame
    void Update()
    {
        speedMult += Time.deltaTime * 0.1f;
        timer += Time.deltaTime;

        if (timer > obstacleInterval)
        {
            Instantiate(spawnObject, spawnPoint.transform.position, Quaternion.identity);
            
            // Reset timer and generate a new interval 
            timer = 0;
            GenerateInterval();
        }
    }
}
