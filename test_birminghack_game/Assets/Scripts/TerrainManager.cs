using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public Transform player;
    public GameObject chunkPrefab;
    public int initialChunks = 5;
    public float init_x = 0f;
    public float init_y = 0f;
    public float chunkSpacing = 0f; // optional gap

    private List<TerrainChunk> chunks = new List<TerrainChunk>();
    private float lastChunkEndX;
    private float lastChunkEndY;

    void Start()
    {
        lastChunkEndX = init_x;
        lastChunkEndY = init_y;

        // Generate initial chunks
        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // Spawn new chunks ahead
        if (player.position.x > lastChunkEndX - 30f)
        {
            SpawnChunk();
        }

        // Remove chunks behind player
        if (chunks.Count > 0 && chunks[0].GetEndX() < player.position.x - 30f)
        {
            Destroy(chunks[0].gameObject);
            chunks.RemoveAt(0);
        }
    }

    // void SpawnChunk()
    // {
    //     Vector3 spawnPos = new Vector3(lastChunkEndX, lastChunkEndY, 0f);
    //     GameObject obj = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
    //     TerrainChunk chunk = obj.GetComponent<TerrainChunk>();

    //     chunk.Generate(lastChunkEndX, lastChunkEndY, true); // generate relative to local origin

    //     // Update the next spawn position
    //     lastChunkEndX += (chunk.points - 1) * chunk.spacing;
    //     lastChunkEndY = chunk.GetEndHeight() + spawnPos.y; // convert back to world Y

    //     chunks.Add(chunk);
    // }

    void SpawnChunk()
{
    Vector3 spawnPos = new Vector3(lastChunkEndX, lastChunkEndY, 0f);
    GameObject obj = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
    TerrainChunk chunk = obj.GetComponent<TerrainChunk>();

    chunk.Generate(0f, 0f, true); // generate relative to local origin

    // Update the next spawn position using actual world end of the chunk
    lastChunkEndX = chunk.transform.position.x + (chunk.points - 1) * chunk.spacing;
    lastChunkEndY = chunk.GetEndHeight() + chunk.transform.position.y;

    chunks.Add(chunk);
}
}