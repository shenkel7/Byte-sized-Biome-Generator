using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    Material biomeMaterial;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public int width = 50;
    public int height = 50;
    public Texture DesertTex;
    public Texture RainforestTex;
    public Texture TundraTex;
    public Texture TaigaTex;
    public Texture GrasslandTex;
    public Texture ForestTex;
    public Texture SavannaTex;
    private Texture[] textures;

    float amplitude;
    float xSeed;
    float ySeed;
    float precipitation;
    float temperature;

    private enum Biome: int { Rainforest, Savanna, Desert, Grassland, Forest, Taiga, Tundra };
    private Vector2[] biomePoints = new Vector2[]
    {
        new Vector2( 1, 1 ), // Rainforest
        new Vector2( 1, .393f ), // Savanna
        new Vector2( 1, 0 ), // Desert
        new Vector2( .633f, .5f ), // Grassland
        new Vector2( .6f, .036f ), // Forest
        new Vector2( .233f, .179f ), // Taiga
        new Vector2( 0, 0 ), // Tundra
    };

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        biomeMaterial = GetComponent<MeshRenderer>().material;

        // populating texture array
        textures = new Texture[7];
        textures[0] = RainforestTex;
        textures[1] = SavannaTex;
        textures[2] = DesertTex;
        textures[3] = GrasslandTex;
        textures[4] = ForestTex;
        textures[5] = TaigaTex;
        textures[6] = TundraTex;

        // global variables
        amplitude = GlobalVariables.amplitude;
        xSeed = GlobalVariables.xSeed;
        ySeed = GlobalVariables.ySeed;
        precipitation = GlobalVariables.precipitation;
        temperature = GlobalVariables.temperature;

        GenerateTerrain();
        UpdateMesh();
    }

    void GenerateTerrain()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];
        triangles = new int[width * height * 6];
        uvs = new Vector2[vertices.Length];


        int index = 0;
        for(int x = 0; x <= width; x++)
        {
            for(int z = 0; z <= height; z++)
            {
                float y = Mathf.PerlinNoise(x * .1f + xSeed, z * .1f + ySeed) * amplitude;
                y += Mathf.PerlinNoise(x * .3f + xSeed, z * .3f + ySeed) * amplitude / 3;
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

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

    }

    // find closest biomes, get texture and calculate color
    void UpdateBiome()
    {

        biomeMaterial.SetTexture("_MainTex2", DesertTex);
        biomeMaterial.SetColor("_Color2", new Color(1, 0, 0));
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        if(amplitude != GlobalVariables.amplitude || xSeed != GlobalVariables.xSeed || ySeed != GlobalVariables.ySeed)
        {
            amplitude = GlobalVariables.amplitude;
            xSeed = GlobalVariables.xSeed;
            ySeed = GlobalVariables.ySeed;
            GenerateTerrain();
            UpdateMesh();
        }
        if(precipitation != GlobalVariables.precipitation || temperature != GlobalVariables.temperature)
        {
            precipitation = GlobalVariables.precipitation;
            temperature = GlobalVariables.temperature;
            UpdateBiome();
        }
    }

}
