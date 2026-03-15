using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject chunkPrefab;
    public GameManager gm;

    [Header("Animation Settings")]
    public float toggleVisibilitySpeed = 2f;

    [Header("Initial Settings")]
    public int initialChunks = 5;
    public float initX = 0f;
    public float initY = 0f;
    public float chunkSpacing = 0f; // optional gap between chunks

    [Header("Spawn Settings")]
    public float spawnDistanceAhead = 30f;
    public float removeDistanceBehind = 30f;

    private List<TerrainChunk> chunks = new List<TerrainChunk>();

    private float lastChunkEndX;
    private float lastChunkEndY;
    private float perlinX;

    void Start()
    {
        lastChunkEndX = initX;
        lastChunkEndY = initY;
        perlinX = 0f;

        // Generate initial chunks
        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // if (gm.currentState == GameState.Playing)
        // {
        //     foreach (TerrainChunk chunk in chunks)
        //     {
        //         Vector3 destination = chunk.gameObject.transform.position + new Vector3(0, 15, 0);
        //         Vector3.MoveTowards(transform.position, destination, toggleVisibilitySpeed * Time.deltaTime);
        //     }
        // }
        // else
        // {
        //     foreach (TerrainChunk chunk in chunks)
        //     {
        //         Vector3 destination = chunk.gameObject.transform.position + new Vector3(0, -15, 0);
        //         Vector3.MoveTowards(transform.position, destination, toggleVisibilitySpeed * Time.deltaTime);
        //     }
        // }

        // Spawn new chunks ahead
        if (player.position.x > lastChunkEndX - (spawnDistanceAhead * initialChunks))
        {
            SpawnChunk();
        }

        // Remove chunks behind
        while (chunks.Count > 0 && chunks[0].GetEndX() < player.position.x - removeDistanceBehind)
        {
            Destroy(chunks[0].gameObject);
            chunks.RemoveAt(0);
        }
    }

    void SpawnChunk()
    {
        Vector3 spawnPos = new Vector3(lastChunkEndX + chunkSpacing, lastChunkEndY, 0f);

        // GameObject obj = Instantiate(chunkPrefab, spawnPos, Quaternion.identity, transform);
        GameObject obj = Instantiate(chunkPrefab, transform);
        obj.transform.localPosition = new Vector3(lastChunkEndX + chunkSpacing, lastChunkEndY, 0f);

        TerrainChunk chunk = obj.GetComponent<TerrainChunk>();

        // Generate chunk with continuity
        chunk.Generate(perlinX, lastChunkEndY);

        // Update perlinX so the next chunk continues the terrain slope
        perlinX += chunk.GetWidth();

        // Update last chunk end positions for next spawn
        lastChunkEndX = spawnPos.x + chunk.GetWidth();
        lastChunkEndY = chunk.GetEndHeight() + spawnPos.y - lastChunkEndY;

        chunks.Add(chunk);

    }

    void OnDestroy()
    {
        foreach(TerrainChunk chunk in chunks)
        {
            Destroy(chunk);
        }
    }
}