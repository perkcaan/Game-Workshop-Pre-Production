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

    private GameObject _closestCleanable;
    public GameObject ClosestCleanable
    {
        get { return _closestCleanable; }
        protected set { _closestCleanable = value; }
    }

    private List<GameObject> cleanablesMaintainersList;

    protected Heap heap;
    private bool adding = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        cleanablesMaintainersList = new List<GameObject>();
        AddNewCleanables();
    }

    void Update()
    {
        if (heap == null || heap.Head == null)
            return;

        heap.PruneNullTargets();

        if (heap.Head == null)
            return;

        Debug.Log(cleanablesMaintainersList.Count);
        Debug.Log(heap.TargetsList.Count);
        if (cleanablesMaintainersList.Count != heap.TargetsList.Count && !adding)
            StartCoroutine(AddCleanablesTickDown());

        heap.RefreshDistances();
        heap.Heapify();

        ClosestCleanable = heap.Head.NodeObject;
        if (ClosestCleanable == null)
            return;

        Vector2 targetDirection = (Vector2)(ClosestCleanable.transform.position - transform.position);
        float rotationDirection = (Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg) - 90f;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationDirection);
    }

    void AddNewCleanables()
    {

        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allGameObjects)
        {
            
            Trash trashTester = go.GetComponent<Trash>();
            EnemyBase enemyTester = go.GetComponent<EnemyBase>();

            if (trashTester == null && enemyTester == null)
                continue;

            Node node = new Node(go, this.gameObject);

            if (heap != null && heap.TargetsList.Exists(n => n.NodeObject == go))
                continue;

            if (heap == null)
                heap = new Heap(node);
            else
                heap.InsertNode(node);

            cleanablesMaintainersList.Add(go);
        }

        if (heap != null)
            heap.Heapify();
    }
    IEnumerator AddCleanablesTickDown()
    {

        adding = true;
        yield return StartCoroutine(AddCleanablesCorutine());
        adding = false;
    }

    IEnumerator AddCleanablesCorutine()
    {
        cleanablesMaintainersList = new List<GameObject>();
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allGameObjects)
        {
            
            Trash trashTester = go.GetComponent<Trash>();
            EnemyBase enemyTester = go.GetComponent<EnemyBase>();

            if (trashTester == null && enemyTester == null)
                continue;

            Node node = new Node(go, this.gameObject);

            cleanablesMaintainersList.Add(go);

            if (heap != null && heap.TargetsList.Exists(n => n.NodeObject == go))
                continue;

            if (heap == null)
                heap = new Heap(node);
            else
                heap.InsertNode(node);

        }

        if (heap != null)
            heap.Heapify();

        yield return new WaitForEndOfFrame();
    }


    protected class Node
    {
        private Node _parent = null;
        private Node _leftChild = null;
        private Node _rightChild = null;

        private GameObject _nodeObject;
        private Vector3 _nodeObjectLocation;
        private GameObject _radarObject;
        private float _distanceFromRadar;

        public Node Parent
        {
            get { return _parent; }
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
            get { return _distanceFromRadar; }
            private set { _distanceFromRadar = value; }
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

        public Node(GameObject obj, GameObject radarObject)
        {
            _radarObject = radarObject;

            NodeObject = obj;
            Location = (obj != null) ? obj.transform.position : Vector3.zero;
            Distance = (obj != null) ? Vector3.Distance(Location, _radarObject.transform.position) : float.PositiveInfinity;
        }

        public void NodeUpdate()
        {
            if (NodeObject == null)
            {
                Distance = float.PositiveInfinity;
                return;
            }

            Location = NodeObject.transform.position;
            Distance = Vector3.Distance(Location, _radarObject.transform.position);
        }

        public void SiftDown()
        {
            while (true)
            {
                Node smallest = this;

                if (LeftChild != null && LeftChild.NodeObject != null && LeftChild.Distance < smallest.Distance)
                    smallest = LeftChild;

                if (RightChild != null && RightChild.NodeObject != null && RightChild.Distance < smallest.Distance)
                    smallest = RightChild;

                if (smallest == this)
                    return;

                SwapPayloadWith(smallest);
            }
        }

        private void SwapPayloadWith(Node other)
        {
            GameObject tempObj = this.NodeObject;
            Vector3 tempLoc = this.Location;
            float tempDist = this.Distance;

            this.NodeObject = other.NodeObject;
            this.Location = other.Location;
            this.Distance = other.Distance;

            other.NodeObject = tempObj;
            other.Location = tempLoc;
            other.Distance = tempDist;
        }
    }

    protected class Heap
    {
        private List<Node> _targets = new List<Node>();
        private Node _headNode;
        private Node _tailNode;

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

        public Heap(Node node)
        {
            Head = node;
            Tail = node;
            TargetsList.Add(node);
        }

        public void InsertNode(Node node)
        {
            TargetsList.Add(node);

            int index = TargetsList.Count - 1;
            int parentIndex = (index - 1) / 2;

            Node parent = TargetsList[parentIndex];
            node.Parent = parent;

            if (parent.LeftChild == null)
                parent.LeftChild = node;
            else
                parent.RightChild = node;

            Tail = node;
        }

        public void RefreshDistances()
        {
            foreach (Node node in TargetsList)
                node.NodeUpdate();
        }

        public void Heapify()
        {
            for (int i = (TargetsList.Count / 2) - 1; i >= 0; i--)
                TargetsList[i].SiftDown();

            Head = (TargetsList.Count > 0) ? TargetsList[0] : null;
        }

        public void PruneNullTargets()
        {
            bool hasNull = false;
            for (int i = 0; i < TargetsList.Count; i++)
            {
                if (TargetsList[i] == null || TargetsList[i].NodeObject == null)
                {
                    hasNull = true;
                    break;
                }
            }

            if (!hasNull)
                return;

            List<GameObject> survivors = new List<GameObject>();
            for (int i = 0; i < TargetsList.Count; i++)
            {
                if (TargetsList[i] != null && TargetsList[i].NodeObject != null)
                    survivors.Add(TargetsList[i].NodeObject);
            }

            TargetsList.Clear();
            Head = null;
            Tail = null;

            for (int i = 0; i < survivors.Count; i++)
            {
                Node n = new Node(survivors[i], Instance.gameObject);
                if (Head == null)
                {
                    Head = n;
                    Tail = n;
                    TargetsList.Add(n);
                }
                else
                {
                    InsertNode(n);
                }
            }

            if (Head != null)
                Heapify();
        }
    }
}
