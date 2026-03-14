using UnityEngine;
using System.Collections.Generic;

public class TerrainSpawner : MonoBehaviour
{
    public Transform player;
    public float spawnAhead = 80f;
    public float despawnBehind = 40f;
    public float hillHeight = 6f;

    private float nextSpawnX;
    private float noiseSeed;
    private float segWidth = 0.3f;

    // visual segments
    private List<GameObject> visualSegments = new List<GameObject>();

    // collider
    private GameObject colliderObj;
    private EdgeCollider2D edgeCollider;
    private List<Vector2> points = new List<Vector2>();

    void Start()
    {
        noiseSeed = Random.Range(0f, 1000f);
        nextSpawnX = -10f;

        // create the collider object
        colliderObj = new GameObject("TerrainCollider");
        colliderObj.tag = "Ground";
        edgeCollider = colliderObj.AddComponent<EdgeCollider2D>();

        // spawn initial ground
        while (nextSpawnX < 80f)
        {
            SpawnSegment();
        }
        UpdateCollider();
    }

    void Update()
    {
        bool added = false;
        while (nextSpawnX < player.position.x + spawnAhead)
        {
            SpawnSegment();
            added = true;
        }
        if (added) UpdateCollider();

        // remove old visual segments
        for (int i = visualSegments.Count - 1; i >= 0; i--)
        {
            if (visualSegments[i] == null)
            {
                visualSegments.RemoveAt(i);
                continue;
            }
            if (visualSegments[i].transform.position.x < player.position.x - despawnBehind)
            {
                Destroy(visualSegments[i]);
                visualSegments.RemoveAt(i);
            }
        }
    }

    float GetHeight(float x)
    {
        float noise = Mathf.PerlinNoise(x * 0.15f + noiseSeed, 0f);
        return (noise - 0.5f) * hillHeight;
    }

    void SpawnSegment()
    {
        float topY = GetHeight(nextSpawnX);

        // visual column
        float height = 20f;
        float centerY = topY - height * 0.5f;

        GameObject seg = new GameObject("Ground");
        seg.transform.position = new Vector3(nextSpawnX + segWidth * 0.5f, centerY, 0f);
        seg.transform.localScale = new Vector3(segWidth, height, 1f);

        SpriteRenderer sr = seg.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite();
        sr.color = new Color(0.3f, 0.8f, 0.3f);
        sr.sortingOrder = -1;

        visualSegments.Add(seg);

        // add point for edge collider
        points.Add(new Vector2(nextSpawnX, topY));

        nextSpawnX += segWidth;
    }

    void UpdateCollider()
    {
        if (points.Count >= 2)
        {
            edgeCollider.points = points.ToArray();
        }
    }

    private Sprite cachedSprite;
    Sprite MakeSprite()
    {
        if (cachedSprite != null) return cachedSprite;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        cachedSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return cachedSprite;
    }
}
