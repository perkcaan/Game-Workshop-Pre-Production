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

    protected Heap heap;

    [SerializeField] GameObject radarPointer;

    [SerializeField] float proxyTimer = 5.0f;
    [SerializeField] float pointerFadeTimer = 1.0f;

    [Tooltip("A reference to the pointer sprite for the radar, set as an array incase it's a composit of sprites")]
    [SerializeField] SpriteRenderer[] pointerSprite;

    private float _pTimer;

    private bool _inProximity = false;
    private bool _adding = false;
    private bool _fading = false;

    private List<GameObject> _proximityList;

    private int _maintainerCount;
    private int _lastCount;

    private readonly Color _alphaFull = new Color(1f, 1f, 1f, 1f);
    private readonly Color _alphaZero = new Color(1f, 1f, 1f, 0f);


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _pTimer = proxyTimer;
        _proximityList = new List<GameObject>();
        _maintainerCount = 0;
        _lastCount = 0;

        AddNewCleanables();

        StartCoroutine(SetSpriteVisability(pointerSprite, _alphaZero, false));
        StartCoroutine(ItemScan());
    }

    void Start()
    {
        StartCoroutine(StartProxyTimer());
    }


    void Update()
    {

        if (heap == null || heap.Head == null)
            return;

        bool removed = heap.PruneNullTargets();

        if (heap.Head == null)
            return;

        if (removed || (_maintainerCount != _lastCount))
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
        _adding = true;
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        _lastCount = allGameObjects.Length;

        foreach (GameObject go in allGameObjects)
        {
            Trash trashTester = go.GetComponent<Trash>();
            EnemyBase enemyTester = go.GetComponent<EnemyBase>();
            TrashBall tbTester = go.GetComponent<TrashBall>();

            if (trashTester == null && enemyTester == null && tbTester == null)
                continue;

            if (heap != null && heap.TargetsList.Exists(n => n.NodeObject == go))
                continue;

            Node node = new Node(go, this.gameObject);

            if (heap == null)
                heap = new Heap(node);
            else
                heap.InsertNode(node);
        }

        if (heap != null)
            heap.Heapify();
        _adding = false;
    }

    IEnumerator AddCleanablesTickDown()
    {
        if (_adding)
            yield break;

        _adding = true;
        AddNewCleanables();
        _adding = false;
        yield return null;
    }

    IEnumerator StartProxyTimer()
    {
        while (_pTimer >= 0f && !_inProximity)
        {
            _pTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(SetSpriteVisability(pointerSprite, _alphaFull, false));
    }

    IEnumerator ItemScan()
    {
        yield return new WaitForSeconds(3.0f);
        while (true)
        {
            GameObject[] array = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            _maintainerCount = array.Length;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator SetSpriteVisability(SpriteRenderer[] sprite, Color targetAlpha, bool fade)
    {
        _fading = true;

        if (!fade)
        {
            foreach (SpriteRenderer sr in sprite)
                sr.color = targetAlpha;
        }
        else
        {
            Color startColor;
            if (targetAlpha != _alphaFull && targetAlpha != _alphaZero)
            {
                yield break;
            }
            else if (targetAlpha == _alphaZero)
                startColor = _alphaFull;
            else
                startColor = _alphaZero;

            float timer = pointerFadeTimer;
            while (timer >= 0f)
            {
                foreach (SpriteRenderer s in pointerSprite)
                {
                    float t = 1 - (timer / pointerFadeTimer);
                    s.color = startColor + ((targetAlpha - startColor) * t);
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }
        _fading = false;
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;

        Trash trashTester = go.GetComponent<Trash>();
        EnemyBase enemyTester = go.GetComponent<EnemyBase>();
        TrashBall tbTester = go.GetComponent<TrashBall>();

        if (trashTester == null && enemyTester == null && tbTester == null)
            return;
        
        if (!_proximityList.Contains(other.gameObject))
            _proximityList.Add(other.gameObject);

        if (!_fading && !_inProximity)
            StartCoroutine(SetSpriteVisability(pointerSprite, _alphaZero, true));

        _inProximity = true;
        _pTimer = proxyTimer;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        GameObject go = other.gameObject;

        Trash trashTester = go.GetComponent<Trash>();
        EnemyBase enemyTester = go.GetComponent<EnemyBase>();
        TrashBall tbTester = go.GetComponent<TrashBall>();

        if (trashTester == null && enemyTester == null && tbTester == null)
            return;

        _inProximity = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        GameObject go = other.gameObject;

        Trash trashTester = go.GetComponent<Trash>();
        EnemyBase enemyTester = go.GetComponent<EnemyBase>();
        TrashBall tbTester = go.GetComponent<TrashBall>();

        if (trashTester == null && enemyTester == null && tbTester == null)
            return;

        _proximityList.Remove(other.gameObject);

        if (_proximityList.Count  == 0)
        {
            _inProximity = false;
            StartCoroutine(StartProxyTimer());
        }
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

        public bool PruneNullTargets()
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
                return false;

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

            return true;
        }
    }
}
