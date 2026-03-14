using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class TerrainChunk : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int points = 10;
    public float spacing = 1f;
    public float amplitude = 4f;
    public float frequency = 0.2f;
    public float slope = 0.05f;
    public float depth = 10f;

    private SpriteShapeController shape;
    private float[] heights;

    /// <summary>
    /// Generate a chunk relative to its transform.
    /// perlinStartX ensures continuity across chunks.
    /// startY is the height of the first point.
    /// </summary>
    public void Generate(float perlinStartX, float startY)
    {
        shape = GetComponent<SpriteShapeController>();
        var spline = shape.spline;
        spline.Clear();

        heights = new float[points];

        // Generate top points
        for (int i = 0; i < points; i++)
        {
            float localX = i * spacing;
            float y = Mathf.PerlinNoise((perlinStartX + localX) * frequency, 0f) * amplitude - (perlinStartX + localX) * slope;

            if (i == 0) y = startY; // connect first point to previous chunk

            heights[i] = y;

            spline.InsertPointAt(i, new Vector3(localX, y - startY, 0f));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        // Bottom edge for collider
        for (int i = points - 1; i >= 0; i--)
        {
            float localX = i * spacing;
            float y = heights[i] - depth;
            int index = points + (points - 1 - i);
            spline.InsertPointAt(index, new Vector3(localX, y - startY, 0f));
        }

        spline.isOpenEnded = false;
        shape.BakeCollider();
    }

    public float GetEndHeight()
    {
        return heights[heights.Length - 1];
    }

    public float GetEndX()
    {
        return transform.position.x + (points - 1) * spacing;
    }

    public float GetWidth()
    {
        return (points - 1) * spacing;
    }
}