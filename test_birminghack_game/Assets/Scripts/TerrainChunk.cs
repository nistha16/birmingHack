using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class TerrainChunk : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int points = 10;
    public float spacing = 1f;
    public float amplitude = 2f;
    public float frequency = 0.2f;
    public float slope = 0.01f;
    public float depth = 10f;

    private SpriteShapeController shape;
    private float[] heights;

    /// <summary>
    /// Generate a chunk relative to its transform.
    /// perlinStartX ensures continuity across chunks.
    /// startY is the height of the first point.
    /// </summary>
    // public void Generate(float perlinStartX, float startY)
    // {
    //     shape = GetComponent<SpriteShapeController>();
    //     var spline = shape.spline;
    //     spline.Clear();

    //     heights = new float[points];

    //     // Generate top points
    //     for (int i = 0; i < points; i++)
    //     {
    //         float localX = i * spacing;
    //         float y = Mathf.PerlinNoise((perlinStartX + localX) * frequency, 0f) * amplitude - (perlinStartX + localX) * slope;

    //         if (i == 0) y = startY; // connect first point to previous chunk

    //         heights[i] = y;

    //         spline.InsertPointAt(i, new Vector3(localX, y - startY, 0f));
    //         spline.SetTangentMode(i, ShapeTangentMode.Continuous);
    //     }

    //     // Bottom edge for collider
    //     for (int i = points - 1; i >= 0; i--)
    //     {
    //         float localX = i * spacing;
    //         float y = heights[i] - depth;
    //         int index = points + (points - 1 - i);
    //         spline.InsertPointAt(index, new Vector3(localX, y - startY, 0f));
    //     }

    //     spline.isOpenEnded = false;
    //     shape.BakeCollider();
    // }

    // public void Generate(float perlinStartX, float startY)
    // {
    //     shape = GetComponent<SpriteShapeController>();
    //     var spline = shape.spline;
    //     spline.Clear();

    //     heights = new float[points];

    //     for (int i = 0; i < points; i++)
    //     {
    //         float localX = i * spacing;
    //         float worldX = perlinStartX + localX;

    //         // 1. Layered Noise (Octaves)
    //         float baseNoise = Mathf.PerlinNoise(worldX * frequency, 0f);
    //         // float detailNoise = Mathf.PerlinNoise(worldX * frequency * 2.5f, 10f) * 0.2f; // Small bumps
            
    //         // 2. The "Black Slope" Modifier
    //         // This makes the noise "taller" in certain sections for big drops
    //         float steepnessVariation = Mathf.Pow(baseNoise, 2.5f) * amplitude;

    //         // 3. Progressive Slope (The actual downhill angle)
    //         float currentSlope = worldX * slope;

    //         float y = steepnessVariation - currentSlope;

    //         if (i == 0) y = startY; 
    //         heights[i] = y;

    //         // Insert point and set to Continuous
    //         spline.InsertPointAt(i, new Vector3(localX, y - startY, 0f));
    //         spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            
    //         // 4. Manual Tangent Smoothing
    //         // Sets the "handle" length to 1/3 of the spacing for a perfect Bezier curve
    //         float tangentLength = spacing / 3f;
    //         spline.SetLeftTangent(i, new Vector3(-tangentLength, 0, 0));
    //         spline.SetRightTangent(i, new Vector3(tangentLength, 0, 0));
    //     }

    //     // Generate bottom edge... (Same as your previous code)
    //     for (int i = points - 1; i >= 0; i--)
    //     {
    //         float localX = i * spacing;
    //         float y = heights[i] - depth;
    //         int index = points + (points - 1 - i);
    //         spline.InsertPointAt(index, new Vector3(localX, y - startY, 0f));
    //     }

    //     spline.isOpenEnded = false;
    //     shape.BakeCollider();
    // }

    [Header("Macro Settings")]
    public float macroFrequency = 0.01f; // Controls how often "Biomes" change
    public float macroExponent = 1.0f;    // Higher = sharper transitions between flat/steep

    public void Generate(float perlinStartX, float startY)
    {
        shape = GetComponent<SpriteShapeController>();
        var spline = shape.spline;
        spline.Clear();

        heights = new float[points];

        for (int i = 0; i < points; i++)
        {
            float localX = i * spacing;
            float worldX = perlinStartX + localX;

            // 1. THE MASTER LAYER (0 to 1)
            // Determines if we are in a "Flat" or "Black Run" zone
            float macroValue = Mathf.PerlinNoise(worldX * macroFrequency, 50f);
            macroValue = Mathf.Pow(macroValue, macroExponent); 

            // 2. FLAT PROFILE CALCULATIONS
            float flatHeight = Mathf.PerlinNoise(worldX * frequency, 0f) * (amplitude * 0.2f);
            float flatSlope = worldX * (slope * 0.5f);
            float yFlat = flatHeight - flatSlope;

            // 3. BLACK RUN PROFILE CALCULATIONS
            float steepHeight = Mathf.PerlinNoise(worldX * frequency * 1.5f, 100f) * (amplitude * 1.0f);
            float steepSlope = worldX * (slope * 1.0f); // Massive downhill push
            float ySteep = steepHeight - steepSlope;

            // 4. THE BLEND
            float y = Mathf.Lerp(yFlat, ySteep, macroValue);

            if (i == 0) y = startY;
            heights[i] = y;

            // Add point to spline
            spline.InsertPointAt(i, new Vector3(localX, y - startY, 0f));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            
            // Manual Tangent: Essential for smooth skiing at high speeds
            float tangentLength = spacing / 2.5f; 
            spline.SetLeftTangent(i, new Vector3(-tangentLength, 0, 0));
            spline.SetRightTangent(i, new Vector3(tangentLength, 0, 0));
        }

        // (Bottom edge generation remains the same)
        FinalizeShape(startY);
    }

    // private void FinalizeShape(float startY)
    // {
    //     var spline = shape.spline;

    //     // 1. Create the "Underground" geometry
    //     // We go backwards from the last point to the first to close the loop
    //     for (int i = points - 1; i >= 0; i--)
    //     {
    //         float localX = i * spacing;
    //         // 'depth' determines how thick the mountain is
    //         float y = heights[i] - depth; 
            
    //         int index = points + (points - 1 - i);
    //         spline.InsertPointAt(points, new Vector3(localX, y - startY, 0f));

    //         // We want the bottom to be jagged/linear, not curvy like the top
    //         spline.SetTangentMode(points, ShapeTangentMode.Linear);
    //     }

    //     // 2. Close the shape so the fill texture appears
    //     spline.isOpenEnded = false;

    //     // 3. Update the Edge Collider 2D to match the new points
    //     shape.BakeCollider();
    // }

    private void FinalizeShape(float startY)
{
    var spline = shape.spline;

    // We already have 'points' number of points (indices 0 to points-1)
    // Now we add the bottom edge in REVERSE order to close the loop properly.
    for (int i = points - 1; i >= 0; i--)
    {
        float localX = i * spacing;
        // Ensure 'depth' is large enough to stay below the lowest valley
        float y = heights[i] - depth; 
        
        // Always add to the very end of the spline list
        int newIndex = spline.GetPointCount();
        spline.InsertPointAt(newIndex, new Vector3(localX, y - startY, 0f));
        
        // IMPORTANT: Bottom points should be Linear so they don't curve
        spline.SetTangentMode(newIndex, ShapeTangentMode.Linear);
    }

    // This closes the gap between the last bottom point and the first top point
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