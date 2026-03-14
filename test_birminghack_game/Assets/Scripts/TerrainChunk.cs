using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class TerrainChunk : MonoBehaviour
{
    public int points = 10;
    public float spacing = 1f;
    public float amplitude = 4f;
    public float frequency = 0.2f;
    public float slope = 0.05f;
    public float depth = 10f;

    private SpriteShapeController shape;
    private float[] heights;

    public void Generate(float startX, float startY, bool useLocalSpace = true)
    {
        shape = GetComponent<SpriteShapeController>();
        var spline = shape.spline;
        spline.Clear();

        heights = new float[points];

        for (int i = 0; i < points; i++)
        {
            float x = i * spacing; // local x
            float y = Mathf.PerlinNoise((startX + x) * frequency, 0) * amplitude - (startX + x) * slope;
            if (i == 0) y = startY;

            heights[i] = y;

            if (useLocalSpace)
                spline.InsertPointAt(i, new Vector3(x, y - startY, 0)); // local coords
            else
                spline.InsertPointAt(i, new Vector3(startX + x, y, 0)); // world coords

            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        // bottom edge
        for (int i = points - 1; i >= 0; i--)
        {
            float x = i * spacing;
            float y = heights[i] - depth;

            int index = points + (points - 1 - i);
            if (useLocalSpace)
                spline.InsertPointAt(index, new Vector3(x, y - startY, 0));
            else
                spline.InsertPointAt(index, new Vector3(startX + x, y, 0));
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
}

