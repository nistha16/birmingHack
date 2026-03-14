using UnityEngine;
using UnityEngine.U2D;

public class TerrainGenerator : MonoBehaviour
{
    public int pointCount = 40;
    public float spacing = 5f;

    public float amplitude = 4f;
    public float frequency = 0.2f;
    public float slope = 0.05f;

    public float depth = 10f;

    SpriteShapeController shape;

    void Start()
    {
        shape = GetComponent<SpriteShapeController>();
        GenerateTerrain();
    }

    float GetHeight(float x)
    {
        return Mathf.PerlinNoise(x * frequency, 0) * amplitude - x * slope;
    }

    void GenerateTerrain()
    {
        var spline = shape.spline;
        spline.Clear();

        float[] heights = new float[pointCount];

        // Generate top points
        for (int i = 0; i < pointCount; i++)
        {
            float x = i * spacing;
            float y = GetHeight(x);

            heights[i] = y;

            spline.InsertPointAt(i, new Vector3(x, y, 0));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        // Set tangents for smooth curves
        for (int i = 1; i < pointCount - 1; i++)
        {
            float prev = heights[i - 1];
            float next = heights[i + 1];

            float dy = next - prev;

            Vector3 tangent = new Vector3(spacing, dy, 0) * 0.3f;

            spline.SetRightTangent(i, tangent);
            spline.SetLeftTangent(i, -tangent);
        }

        // Bottom edge
        for (int i = pointCount - 1; i >= 0; i--)
        {
            float x = i * spacing;
            float y = heights[i] - depth;

            int index = pointCount + (pointCount - 1 - i);
            spline.InsertPointAt(index, new Vector3(x, y, 0));
        }

        spline.isOpenEnded = false;

        shape.BakeCollider();
    }
}