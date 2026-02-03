using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BHeap<T> : MonoBehaviour
{
    private List<Node> _heap = new List<Node>();

    private class Node
    {
        public T Item;
        public float Priority;

        public Node(T item, float priority)
        {
            Item = item;
            Priority = priority;
        }
    }

    public int Count => _heap.Count;

    public bool IsEmpty => _heap.Count == 0;

    public void Enqueue(T item, float priority)
    {
        _heap.Add(new Node(item, priority));
        HeapifyUp(_heap.Count - 1);
    }

    public T Dequeue()
    {
        T item = _heap[0].Item;

        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);

        if (_heap.Count > 0)
            HeapifyDown(0);

        return item;
    }

    public T Peek()
    {
        return _heap[0].Item;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (_heap[index].Priority >= _heap[parent].Priority)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = _heap.Count - 1;

        while (true)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left <= lastIndex &&
                _heap[left].Priority < _heap[smallest].Priority)
            {
                smallest = left;
            }

            if (right <= lastIndex &&
                _heap[right].Priority < _heap[smallest].Priority)
            {
                smallest = right;
            }

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        Node temp = _heap[a];
        _heap[a] = _heap[b];
        _heap[b] = temp;
    }
}
