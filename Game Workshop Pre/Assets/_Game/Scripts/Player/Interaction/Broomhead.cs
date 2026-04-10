using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Broomhead : MonoBehaviour
{
    private Collider2D _collider;
    private BroomSweepHandler _handler;

    public bool Active {
        get { return _collider.enabled; }
        set { _collider.enabled = value; }
    }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void Initialize(BroomSweepHandler handler)
    {
        _handler = handler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }


}