using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BroomPokebox : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        _handler.OnPokeHit(collider);
    }


}