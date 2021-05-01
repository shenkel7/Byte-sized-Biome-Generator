﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

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


        int index = 0;
        for(int x = 0; x <= width; x++)
        {
            for(int z = 0; z <= height; z++)
            {
                float y = Mathf.PerlinNoise(x * .1f, z * .1f) * amplitude;
                y += Mathf.PerlinNoise(x * .3f, z * .3f) * amplitude / 3;
                vertices[index] = new Vector3(x, y, z);
                index++;
            }
        }

        int tri = 0;
        int vertex = 0;
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                triangles[tri] = vertex;
                triangles[tri + 1] = vertex + 1;
                triangles[tri + 2] = vertex + width + 1;
                triangles[tri + 3] = vertex + width + 1;
                triangles[tri + 4] = vertex + 1;
                triangles[tri + 5] = vertex + width + 2;
                vertex++;
                tri += 6;
            }
            vertex++;
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        if(amplitude != GlobalVariables.amplitude)
        {
            amplitude = GlobalVariables.amplitude;
            GenerateTerrain();
            UpdateMesh();
        }
    }

}
