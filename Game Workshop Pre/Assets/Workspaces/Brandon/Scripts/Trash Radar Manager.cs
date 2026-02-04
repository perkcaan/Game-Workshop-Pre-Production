using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashRadarManager : MonoBehaviour
{
    private static TrashRadarManager _instance;
    public static TrashRadarManager Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }

    private List<GameObject> _targetList = new List<GameObject>();
    public List<GameObject> TargetsList 
    {
        get { return _targetList; }
        private set { _targetList = value; }
    }

    protected Heap heap = new Heap();

    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject gameObject in allGameObjects)
        {
            Trash trashTester = gameObject.GetComponent<Trash>();
            if (trashTester != null)
            {
                TargetsList.Add(trashTester.gameObject);
                continue;
            }
            EnemyBase enemyTester = gameObject.GetComponent<EnemyBase>();
            if (enemyTester != null)
            {
                TargetsList.Add(enemyTester.gameObject);
                continue;
            }
        }
    }
    protected class Node : MonoBehaviour
    {

        private Node _parent = null;
        private Node _leftChild = null;
        private Node _rightChild = null;
        private GameObject _nodeObject;
        private Vector3 _nodeObjectLocation;
        private GameObject _radarObject = Instance.gameObject;
        private float _distanceFromPlayer;

        public Node Parent { 
            get { return _parent;}
            set { _parent = value; }
        }

        public Node LeftChild
        {
            get { return _leftChild; }
            set { _leftChild = value; }
        }

        public Node RightChild
        {
            get { return _rightChild; }
            set { _rightChild = value; }
        }

        public float Distance
        {
            get { return _distanceFromPlayer; }
            private set { _distanceFromPlayer = value; }
        }

        public GameObject NodeObject
        {
            get { return _nodeObject; }
            private set { _nodeObject = value; }
        }

        protected Vector3 Location
        {
            get { return _nodeObjectLocation; }
            private set { _nodeObjectLocation = value; }
        }

        protected Node(GameObject obj)
        {
            NodeObject = obj;
            Location= obj.transform.position;
            Distance = Vector3.Distance(Location, _radarObject.transform.position);
        }

        private void Update()
        {
            Location = NodeObject.transform.position;
            Distance = Vector3.Distance(Location, _radarObject.transform.position);
            CompareDistances(Distance, LeftChild, RightChild);
            if (Parent == null)
                Instance.heap.MakeHead(this);
        }

        private void CompareDistances(float distance, Node LeftChild, Node RightChild)
        {
            if (distance > LeftChild.Distance)
                RotateLeft(LeftChild);
            else if (distance > RightChild.Distance)
                RotateRight(RightChild);
        }

        private void RotateLeft(Node LeftChild)
        {
            Node temp = this;
            this.Parent = LeftChild;
            LeftChild.Parent = temp.Parent;
            this.LeftChild = LeftChild.LeftChild;
            this.RightChild = LeftChild.RightChild;
            LeftChild.LeftChild = temp.RightChild;
            LeftChild.RightChild = this;
        }

        private void RotateRight(Node RightChild)
        {
            Node temp = this;
            this.Parent = RightChild;
            RightChild.Parent = temp.Parent;
            this.RightChild = RightChild.RightChild;
            this.LeftChild = RightChild.LeftChild;
            RightChild.RightChild = temp.LeftChild;
            RightChild.LeftChild = this;
        }
    }

    protected class Heap
    {
        private List<Node> _targets = new List<Node>();
        private Node _headNode;
        private Node _tailNode;
        private int _depth;
        private Node[] depthQueue;
        private static int nodeNumber;
        
        public List<Node> TargetsList
        {
            get { return _targets; }
            private set { _targets = value; }
        }

        public Node Head
        {
            get { return _headNode; }
            private set { _headNode = value; }
        }

        protected Node Tail
        {
            get { return _tailNode; }
            private set { _tailNode = value; }
        }

        protected int Depth
        {
            get { return _depth; }
            private set { _depth = value; }
        }

        void InsertNode(Node node)
        {
            if (Tail.LeftChild == null)
            {
                Tail.LeftChild = node;
                nodeNumber++;
                return;
            }
            else if (Tail.RightChild == null)
            {
                Tail.RightChild = node;
                nodeNumber++;
                return;
            }

            if (nodeNumber == depthQueue.Length)
            {
                ConstructNewQueue();
                nodeNumber = 0;
                InsertNode(Tail);
            }

            MakeTail(depthQueue[++nodeNumber]);
            InsertNode(Tail);
        }

        void ConstructNewQueue()
        {
            Node[] tempQueue = new Node[nodeNumber * 2];
            for (int i = depthQueue.Length; i > 0; i--)
            {
                tempQueue[(i * 2) - 1] = depthQueue[i - 1].RightChild;
                tempQueue[(i * 2) - 2] = depthQueue[i - 1].LeftChild;
            }
            depthQueue = tempQueue;
            MakeTail(depthQueue[0]);
        }

        public void MakeHead(Node node)
        {
            Head = node;
        }

        void MakeTail(Node node)
        {
            Tail = node;
        }

    }
}

