using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

// transformation data of turtle
public class Transformation {
    public Vector3 position;
    public Quaternion rotation;
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

    // customizable globals
    private float r1;       // contraction ratios 
    private float r2;
    private float a0;       // branching angles
    private float a2;
    private float d;        // divergence angles
    private float wr;       // trunk width decrease rate
    private int iterations; // number of iterations to generate tree

    private Vector3 startingPosition;
    private Quaternion startingRotation;

    // List of characters in L system
    List<Node> nodes;
    // List of branches to delete upon update
    List<GameObject> branches;
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

    // Start is called before the first frame update
    void Start()
    {
        setGlobals(); 
        Debug.Log("created monotree with iterations" + iterations);
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        nodes = new List<Node>();
        branches = new List<GameObject>();


        var scale = new Vector3(1f, 1f, .5f);
        Branch.transform.localScale = scale;

        nodes.Add(new Node('A', 2f, .3f));
        transformStack = new Stack<Transformation>();
        
        GenerateTree();
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
    private void GenerateTree() {

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
                        rotation = transform.rotation
                    });
                    break;
                case ']':
                    Transformation t = transformStack.Pop();
                    transform.position = t.position;
                    transform.rotation = t.rotation;
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
    }

    void Update() {
        if(TreeGlobals.updated) {
            // destroy old branches
            foreach(GameObject b in branches) {
                Destroy(b);
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
            nodes = new List<Node>();
            nodes.Add(new Node('A', 2f, .3f));

            GenerateTree();
            TreeGlobals.updated = false;
        }
    }
}
