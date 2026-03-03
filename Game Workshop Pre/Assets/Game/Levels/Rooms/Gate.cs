using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private bool _isGateDown = false;
    private List<Room> _roomsClosingGate = new List<Room>();

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }

    public void Open(Room room)
    {
        if (_roomsClosingGate.Contains(room)) _roomsClosingGate.Remove(room);
        if (!_isGateDown) return; //already open

        if (_roomsClosingGate.Count <= 0)
        {
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            _isGateDown = false;
        }
    }

    public void Close(Room room)
    {
        if (!_roomsClosingGate.Contains(room)) _roomsClosingGate.Add(room);
        if (_isGateDown) return; //already closed
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
        _isGateDown = true;
    }
}
