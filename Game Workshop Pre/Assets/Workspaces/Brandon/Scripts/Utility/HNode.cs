using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HNode : MonoBehaviour
{
    //Heap Node
    private HNode _lChild;
    private HNode _rChild;
    private HNode _parent;
    private GameObject _obj;
    private Vector2 _location;
    private int _depth;

    public HNode(GameObject obj, int depth)
    {
        Object = obj;
        Location = obj.transform.position;
        LeftChild = null;
        RightChild = null;
        Parent = null;
        Depth = depth;
    }

    public GameObject Object
    {
        get { return _obj; }
        set { Object = value; }
    }

    public GameObject LeftChild
    {
        get { return _lChild.Object; }
        private set { LeftChild = value; }
    }

    public GameObject RightChild
    {
        get { return _rChild.Object; }
        private set { RightChild = value; }
    }

    public GameObject Parent
    {
        get { return _parent.Object; }
        private set { Parent = value; }
    }

    public int Depth
    {
        get { return _depth; }
        set { _depth = value; }
    }

    public Vector2 Location
    {
        get { return _location; }
        private set { _location = value; }
    }

}