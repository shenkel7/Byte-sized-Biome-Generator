using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int width = 50;
    public int height = 50;
    float amplitude;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        amplitude = GlobalVariables.amplitude;

        GenerateTerrain();
        UpdateMesh();
    }

    void GenerateTerrain()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];
        triangles = new int[width * height * 6];
        colors = new Color[vertices.Length];


        int index = 0;
        for(int x = 0; x <= width; x++)
        {
            for(int z = 0; z <= height; z++)
            {
                float y = Mathf.PerlinNoise(x * .1f, z * .1f) * amplitude;
                y += Mathf.PerlinNoise(x * .3f, z * .3f) * amplitude / 2;
                vertices[index] = new Vector3(x, y, z);
                index++;
            }
        }

        int tri = 0;
        int vertex = 0;
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                triangles[tri] = vertex;
                triangles[tri + 1] = vertex + 1;
                triangles[tri + 2] = vertex + width + 1;
                triangles[tri + 3] = vertex + 1;
                triangles[tri + 4] = vertex + width + 1;
                triangles[tri + 5] = vertex + width + 2;
                vertex++;
                tri += 6;
            }
            vertex++;
        }

        // create new colors array where the colors will be created.

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        amplitude = GlobalVariables.amplitude;
        GenerateTerrain();
        UpdateMesh();
    }

}
