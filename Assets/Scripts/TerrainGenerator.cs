using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] public GameObject MonoTree; 
    [SerializeField] public int MaxTrees; 
    

    Mesh mesh;
    Material biomeMaterial;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public int width = 50;
    public int height = 50;

    // biome textures
    public Texture DesertTex;
    public Texture RainforestTex;
    public Texture TundraTex;
    public Texture TaigaTex;
    public Texture GrasslandTex;
    public Texture ForestTex;
    public Texture SavannaTex;
    public Texture SnowTex;

    // biome colors
    public Color DesertCol;
    public Color RainforestCol;
    public Color TundraCol;
    public Color TaigaCol;
    public Color GrasslandCol;
    public Color ForestCol;
    public Color SavannaCol;
    public Color SnowCol;

    private Texture[] textures;
    private Color[] colors;

    float amplitude;
    float xSeed;
    float ySeed;
    float precipitation;
    float temperature;

    // trees
    List<GameObject> trees;

    private Vector2[] biomePoints = new Vector2[]
    {
        new Vector2( 1, 1 ), // Rainforest
        new Vector2( 1, .393f ), // Savanna
        new Vector2( 1, 0 ), // Desert
        new Vector2( .633f, .5f ), // Grassland
        new Vector2( .6f, .036f ), // Forest
        new Vector2( .233f, .179f ), // Taiga
        new Vector2( 0, 0 ), // Tundra
        new Vector2( 0, 1 ) // Snow
    };

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        biomeMaterial = GetComponent<MeshRenderer>().material;

        // populating texture array
        textures = new Texture[8];
        textures[0] = RainforestTex;
        textures[1] = SavannaTex;
        textures[2] = DesertTex;
        textures[3] = GrasslandTex;
        textures[4] = ForestTex;
        textures[5] = TaigaTex;
        textures[6] = TundraTex;
        textures[7] = SnowTex;

        // populating color array
        colors = new Color[8];
        colors[0] = RainforestCol;
        colors[1] = SavannaCol;
        colors[2] = DesertCol;
        colors[3] = GrasslandCol;
        colors[4] = ForestCol;
        colors[5] = TaigaCol;
        colors[6] = TundraCol;
        colors[7] = SnowCol;

        // global variables
        amplitude = GlobalVariables.amplitude;
        xSeed = GlobalVariables.xSeed;
        ySeed = GlobalVariables.ySeed;
        precipitation = GlobalVariables.precipitation;
        temperature = GlobalVariables.temperature;

        trees = new List<GameObject>();
        GenerateTerrain();
        UpdateMesh();
    }

    void GenerateTerrain()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];
        triangles = new int[width * height * 6];
        uvs = new Vector2[vertices.Length];

        // destroy old trees
        for(int i = 0; i < trees.Count; i++){
            trees[i].GetComponent<Monotree>().destroyBranches();
            Destroy(trees[i]);            
        }

        trees = new List<GameObject>();

        int numTrees = 0;
        int index = 0;
        int space = 0;
        for(int x = 0; x <= width; x++)
        {
            for(int z = 0; z <= height; z++)
            {
                float y = Mathf.PerlinNoise(x * .1f + xSeed, z * .1f + ySeed) * amplitude;
                y += Mathf.PerlinNoise(x * .3f + xSeed, z * .3f + ySeed) * amplitude / 3;
                vertices[index] = new Vector3(x, y, z);
                ++index;
                Vector3 treePosition = new Vector3(x,y,z) + transform.position;
                if(numTrees < MaxTrees && space > 235) {
                    Debug.Log("created tree at pos " + x + " " + y + " " + z);
                    GameObject tree = Instantiate(MonoTree, treePosition, transform.rotation);
                    trees.Add(tree);
                    // tree.GetComponent<MonoTree>().initialize(treePosition);
                    ++numTrees;
                    space = 0;
                }
                space++;
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
        // for each point, calculate distance
        // texture = biome closest distance
        // color = lerp 3 closest color (bounds)

        Vector2 point = new Vector2(temperature, precipitation);
        
        float minDist = Vector2.Distance(biomePoints[0], point);
        float minDist2 = Vector2.Distance(biomePoints[1], point);
        float minDist3 = 100; // will definitely change 
        int minIndex = 0;
        int minIndex2 = 1;
        int minIndex3 = 100; // will definitely change

        if (minDist > minDist2)
        {
            float temp = minDist;
            minDist = minDist2;
            minDist2 = temp;
            minIndex = 1;
            minIndex2 = 0;
        }


        for(int i = 1; i < biomePoints.Length; i++)
        {
            float tempDist = Vector2.Distance(biomePoints[i], point);
            if(tempDist <= minDist3)
            {
                if(tempDist <= minDist2)
                {
                    if(tempDist <= minDist)
                    {
                        minDist3 = minDist2;
                        minDist2 = minDist;
                        minDist = tempDist;
                        minIndex3 = minIndex2;
                        minIndex2 = minIndex;
                        minIndex = i;
                    } else
                    {
                        minDist3 = minDist2;
                        minDist2 = tempDist;
                        minIndex3 = minIndex2;
                        minIndex2 = i;
                    }
                } else
                {
                    minDist3 = tempDist;
                    minIndex3 = i;
                }
            }
        }


        biomeMaterial.SetTexture("_MainTex2", textures[minIndex]);
        Color c = Color.Lerp(colors[minIndex2], colors[minIndex3], minDist3 / (minDist2 + minDist3));
        c = Color.Lerp(colors[minIndex], c, minDist);
        biomeMaterial.SetColor("_Color2", c);
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
