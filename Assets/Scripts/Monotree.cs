using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

// transformation data of turtle
public class Transformation {
    public Vector3 position;
    public Quaternion rotation;
    public int depth;
}

// struct to store a character/rule and its data
public struct Node {

    public Node(char n, float i = -1f, float j = -1f) {
        name = n;
        l = i;
        w = j;
    }
    
    public char name;
    public float l;
    public float w;
}

public class Monotree : MonoBehaviour
{
    // public void initialize(Vector3 initialPosition) {
    //     startingPosition = initialPosition;
    // }

    [SerializeField] private GameObject Branch; // branch object to instantiate
    [SerializeField] private GameObject Leaf;

    // customizable globals
    private float r1;       // contraction ratios 
    private float r2;
    private float a0;       // branching angles
    private float a2;
    private float d;        // divergence angles
    private float wr;       // trunk width decrease rate
    private int iterations; // number of iterations to generate tree
    private float trunkLength;
    private float treeLength;
    private float treeWidth;
    private bool toggleLeaves;
    private float leafSize;
    private Material trunkMat;
    private Material leafMat;

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private int currentDepth;
    // List of characters in L system
    List<Node> nodes;
    // List of branches to delete upon update
    List<GameObject> branches;
    List<GameObject> leafs = new List<GameObject>();
    // Stack of transformation data
    private Stack<Transformation> transformStack;

    // sets global variables from TreeGlobals
    void setGlobals() {
        iterations = TreeGlobals.iterations;
        r1 = TreeGlobals.r1;
        r2 = TreeGlobals.r2;
        a0 = TreeGlobals.a0;
        a2 = TreeGlobals.a2;
        d = TreeGlobals.d;
        wr = TreeGlobals.wr;
    }

    public void Initialize(int biome) {
        // Debug.Log("created monotree with iterations" + iterations);
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        transform.Rotate(Vector3.up * Random.Range(0f, 360f), Space.Self);
        transform.Rotate(Vector3.right * Random.Range(0f, 10f), Space.Self);
        nodes = new List<Node>();
        branches = new List<GameObject>();
        transformStack = new Stack<Transformation>();
        List<GameObject> leafs = new List<GameObject>();


        // standard tree parameters
        iterations = Random.Range(1, 6);
        // iterations = 4;
        trunkLength = 2f;
        treeLength = Random.Range(2f, 4f);
        treeWidth = .3f;
        r1 = 0.9f;
        r2 = 0.6f;
        a0 = 45f;
        a2 = 45f;
        d = 137.5f;
        wr = 0.707f;
        toggleLeaves = true;
        leafSize = 0.9f;
        Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(164f/255f, 81f/255f, 27f/255f);
        Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(30f/255f, 164f/255f, 44f/255f, 188f/255f);
        
        switch(biome) {
            case 0: // Rainforest
                trunkLength = 4f;
                treeLength = Random.Range(2f, 6f);
                treeWidth = .8f;
                leafSize = 2.0f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(79f/255f, 79f/255f, 0f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(6f/255f, 92f/255f, 20f/255f, 188f/255f);
                nodes.Add(new Node('A', treeLength, treeWidth));
                break;            
            case 1: // Savanna
                r1 = .9f;
                r2 = .8f;
                a0 = 35f;
                a2 = 35f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(135f/255f, 163f/255f, 98f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(6f/255f, 92f/255f, 20f/255f, 188f/255f);
                nodes.Add(new Node('E', treeLength, treeWidth));
                break;
            case 2: // Desert
                iterations = Random.Range(1,3);
                r1 = .4f;
                r2 = .4f;
                a0 = 90f;
                a2 = 90f;
                wr = 0.9f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(53f/255f, 230f/255f, 88f/255f);
                toggleLeaves = false;
                nodes.Add(new Node('A', treeLength, treeWidth));
                break;
            case 3: // Grassland
                nodes.Add(new Node('A', treeLength, treeWidth));
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(164f/255f, 81f/255f, 27f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(30f/255f, 164f/255f, 44f/255f, 188f/255f);
                break;
            case 4: // Forest
                a0 = 30f;
                a2 = 30f;
                d = 120f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(164f/255f, 81f/255f, 27f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(30f/255f, 164f/255f, 44f/255f, 188f/255f);
                nodes.Add(new Node('A', treeLength, treeWidth));
                break;
            case 5: // Taiga
                r1 = .9f;
                r2 = .5f;
                a0 = 5f;
                a2 = 65f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(135f/255f, 163f/255f, 98f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(2f/255f, 66f/255f, 48f/255f, 188f/255f);
                nodes.Add(new Node('D', treeLength, treeWidth));
                break;
            case 6: // Tundra
                r1 = .9f;
                r2 = .7f;
                a0 = 5f;
                a2 = 65f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(59f/255f, 28f/255f, 0f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(0f/255f, 38f/255f, 24f/255f, 188f/255f);
                nodes.Add(new Node('D', treeLength, treeWidth));
                break;
            case 7: // Snow
                r1 = .7f;
                r2 = .5f;
                a0 = 10f;
                a2 = 65f;
                treeLength = .8f;
                treeWidth = .1f;
                Branch.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(140f/255f, 122f/255f, 112f/255f);
                Leaf.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(230f/255f, 116f/255f, 55f/255f, 188f/255f);
                nodes.Add(new Node('D', treeLength, treeWidth));
                break;
            default: 
                break;
        }

        var scale = new Vector3(1f, 1f, .5f);
        Branch.transform.localScale = scale;

        GenerateTree();
    }

    // Start is called before the first frame update
    void Start()
    {
        // setGlobals(); 
        
        
    }

    // rule A - adds a set of characters with values
    private void addA(Node A, List<Node> nodesList) {
        nodesList.Add(new Node('!', A.w));
        nodesList.Add(new Node('F', A.l));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('&', a0));
        nodesList.Add(new Node('B', A.l * r2, A.w * wr));
        nodesList.Add(new Node(']'));
        nodesList.Add(new Node('/', d));
        nodesList.Add(new Node('A', A.l * r1, A.w * wr));
    }

    // rule B - adds a set of characters with values
    private void addB(Node B, List<Node> nodesList) {
        nodesList.Add(new Node('!', B.w));
        nodesList.Add(new Node('F', B.l));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('-', a2));
        nodesList.Add(new Node('$'));
        nodesList.Add(new Node('C', B.l * r2, B.w * wr));
        nodesList.Add(new Node(']'));
        nodesList.Add(new Node('C', B.l * r1, B.w * wr));
    }

     // rule C - adds a set of characters with values
    private void addC(Node C, List<Node> nodesList) {
        nodesList.Add(new Node('!', C.w));
        nodesList.Add(new Node('F', C.l));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('+', a2));
        nodesList.Add(new Node('$'));
        nodesList.Add(new Node('B', C.l * r2, C.w * wr));
        nodesList.Add(new Node(']'));
        nodesList.Add(new Node('B', C.l * r1, C.w * wr));
    }

    // rule  - adds a set of characters with values
    private void addD(Node D, List<Node> nodesList) {
        nodesList.Add(new Node('!', D.w));
        nodesList.Add(new Node('F', D.l));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('&', a0));
        nodesList.Add(new Node('E', D.l * r1, D.w * wr));
        nodesList.Add(new Node(']'));
        nodesList.Add(new Node('/', 180f));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('&', a2));
        nodesList.Add(new Node('E', D.l * r2, D.w * wr));
        nodesList.Add(new Node(']'));
    }

    private void addE(Node E, List<Node> nodesList) {
        nodesList.Add(new Node('!', E.w));
        nodesList.Add(new Node('F', E.l));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('+', a0));
        nodesList.Add(new Node('$'));
        nodesList.Add(new Node('E', E.l * r1, E.w * wr));
        nodesList.Add(new Node(']'));
        nodesList.Add(new Node('['));
        nodesList.Add(new Node('-', a2));
        nodesList.Add(new Node('$'));
        nodesList.Add(new Node('E', E.l * r2, E.w * wr));
        nodesList.Add(new Node(']'));
    }

    private void GenerateTree() {
        currentDepth = 0;

        // Generate string of characters
        for(int i = 0; i < iterations; i++) {
            List<Node> newNodes = new List<Node>();

            foreach(Node n in nodes) {
                // follow rule or add character
                switch(n.name) {
                    case 'A':
                        addA(n, newNodes);
                        break;
                    case 'B':
                        addB(n, newNodes);
                        break;
                    case 'C':
                        addC(n, newNodes);
                        break;
                    case 'D':
                        addD(n, newNodes);
                        break;
                    case 'E':
                        addE(n, newNodes);
                        break;
                    default:
                        newNodes.Add(n);
                        break;
                }
            }
            nodes = newNodes;
        }

        // Debug.Log(cur);
        
        foreach(Node n in nodes) {
            switch(n.name) {
                case 'F':
                    Vector3 initialPos = transform.position;
                    transform.Translate(Vector3.up * n.l);
                    
                    GameObject branch = Instantiate(Branch, initialPos + (transform.position-initialPos)/2, transform.rotation);
                    var scale = new Vector3(branch.transform.localScale.x, n.l/2, branch.transform.localScale.z);
                    branch.transform.localScale = scale;
                    branches.Add(branch);
                    if(toggleLeaves && currentDepth > 1) {
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                        GameObject leaf = Instantiate(Leaf, transform.position, randomRotation);
                        var leafScale = new Vector3(leafSize, leafSize, leafSize);
                        leaf.transform.localScale = leafScale;
                        leafs.Add(leaf);
                    }
                    currentDepth++;
                    break;
                case '+':
                    transform.Rotate(Vector3.forward * n.l, Space.Self);
                    break;
                case '-':
                    transform.Rotate(Vector3.back * n.l, Space.Self);
                    break;
                case '&':
                    transform.Rotate(Vector3.left * n.l, Space.Self);
                    break;
                case '^':
                    transform.Rotate(Vector3.right * n.l, Space.Self);
                    break;
                case '\\':
                    transform.Rotate(Vector3.down * n.l, Space.Self);
                    break;
                case '/':
                    transform.Rotate(Vector3.up * n.l, Space.Self);
                    break;
                case '[':
                    transformStack.Push(new Transformation()
                    {
                        position = transform.position,
                        rotation = transform.rotation,
                        depth = currentDepth
                    });
                    break;
                case ']':
                    Transformation t = transformStack.Pop();
                    transform.position = t.position;
                    transform.rotation = t.rotation;
                    currentDepth = t.depth;
                    break;
                case '$':
                    // transform.up = Vector3.up;
                    // Vector3 h = transform.forward;
                    // Vector3 v = Vector3.up;
                    // Vector3 l = Vector3.Cross(h,v) / Vector3.Magnitude(Vector3.Cross(h,v));
                    // transform.up = Vector3.Cross(h,l);
                    // transform.up = Vector3.up;
                    transform.Rotate(Vector3.up * Random.Range(15.0f, 70.0f), Space.Self);
                    break;
                case '!':
                    var newScale = new Vector3(n.l, Branch.transform.localScale.y, n.l);
                    Branch.transform.localScale = newScale;
                    // transform.up = Vector3.up;
                    break;

            }
        }
    }

    public void destroyBranches() {
        foreach(GameObject b in branches) {
            Destroy(b);
        }

        foreach(GameObject l in leafs) {
                Destroy(l);
            }
    }

    void Update() {
        if(TreeGlobals.updated) {
            // destroy old branches
            foreach(GameObject b in branches) {
                Destroy(b);
            }

            foreach(GameObject l in leafs) {
                Destroy(l);
            }

            // reset turtle position
            transform.position = startingPosition;
            transform.rotation = startingRotation;
            setGlobals();

            // reset branch scale
            var scale = new Vector3(1f, 1f, .5f);
            Branch.transform.localScale = scale;

            // reset stack / lists
            transformStack = new Stack<Transformation>();
            branches = new List<GameObject>();
            leafs = new List<GameObject>();
            nodes = new List<Node>();
            nodes.Add(new Node('A', treeLength, treeWidth));

            GenerateTree();
            TreeGlobals.updated = false;
        }
    }
}
