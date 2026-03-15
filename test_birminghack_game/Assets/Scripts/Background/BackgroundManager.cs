using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    class Layer
    {
        public float width;
        public Transform[] tiles = new Transform[3];
    }

    private List<Layer> layers = new List<Layer>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            Layer layer = new Layer
            {
                width = sr.bounds.size.x
            };

            layer.tiles[0] = child;

            // create left tile
            Transform left = Instantiate(child, transform);
            left.position = child.position - new Vector3(layer.width, 0, 0);
            layer.tiles[1] = left;

            // create right tile
            Transform right = Instantiate(child, transform);
            right.position = child.position + new Vector3(layer.width, 0, 0);
            layer.tiles[2] = right;

            layers.Add(layer);
        }
    }

    void LateUpdate()
    {
        foreach (Layer layer in layers)
        {
            RecycleTiles(layer);
        }
    }

    void RecycleTiles(Layer layer)
    {
        float cameraX = Camera.main.transform.position.x;

        foreach (Transform tile in layer.tiles)
        {
            if (cameraX > tile.position.x + layer.width)
            {
                tile.position += new Vector3(layer.width * 3f, 0, 0);
            }

            if (cameraX < tile.position.x - layer.width)
            {
                tile.position -= new Vector3(layer.width * 3f, 0, 0);
            }
        }
    }
}